using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SessionLib;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using System.Threading;
using System.Text;

namespace MonitorTest
{
    public class Program
    {
        IConfiguration Config { get; set; }
        IServiceProvider Provider { get; set; }

        ISessionFactory Factory { get; set; }
        IClientSession Session { get; set; }

        public void Main(string[] args)
        {
            Config = new Configuration();
            var services = new ServiceCollection();
            services.AddSessions();
            Provider = services.BuildServiceProvider();
            Factory = Provider.GetService<ISessionFactory>();

            GetConnected();
            RunConnected();
        }

        static string linePat = @"(?<command>[se])\w*\s*(?<content>.*$)?";
        static System.Text.RegularExpressions.Regex lineRegex = new System.Text.RegularExpressions.Regex(linePat);
        static string defaultEndpoint = "ws://localhost:57762/ws";

        void GetConnected()
        {
            while (true)
            {
                Console.Write("Endpoint[" + defaultEndpoint + "]: ");
                var endpoint = Console.ReadLine();
                if (string.IsNullOrEmpty(endpoint))
                    endpoint = defaultEndpoint;

                try
                {
                    this.Session = this.Factory.ConnectAsync(endpoint, CancellationToken.None).Result;
                }
                catch (Exception e)
                {
                    Console.WriteLine("?connect failed: " + e.Message);
                    continue;
                }

                if (Session != null)
                    break;

                Console.WriteLine("?failed to connect to: " + endpoint);
            }

            Session.OnReceive += Session_OnReceive;
        }

        private Task Session_OnReceive(ISession session, ArraySegment<byte> m)
        {
            var message = UTF8Encoding.UTF8.GetString(m.Array, 0, m.Count);
            Console.WriteLine("Received message: " + message);
            return Task.FromResult<bool>(true);
        }

        void RunConnected()
        {
            while (true)
            {
                Console.WriteLine(@"
Options:
  [s]end message
  [e]xit
");
                var line = Console.ReadLine();
                var m = lineRegex.Match(line);
                if (!m.Success)
                {
                    Console.WriteLine("?Could not match command: " + line);
                    continue;
                }

                var command = m.Groups["command"].Value;
                switch (command)
                {
                    case "s":
                        {
                            var content = m.Groups["content"].Value;
                            var bytes = UTF8Encoding.UTF8.GetBytes(content);
                            var seg = new ArraySegment<byte>(bytes);
                            this.Session.SendAsync(seg);
                            break;
                        }
                    case "e":
                        Console.WriteLine("Exiting...");
                        return;
                    default:
                        throw new ApplicationException("unhandled command: " + command);
                }

            }

        }




    }
}
