using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using MessageLib;

namespace MonitorTest
{
    public class Program
    {
        IConfiguration Config { get; set; }
        IServiceProvider Provider { get; set; }

        IMessengerFactory Factory { get; set; }
        IClientMessenger Messenger { get; set; }

        public void Main(string[] args)
        {
            Config = new Configuration();
            var services = new ServiceCollection();
            services.AddMessages();
            Provider = services.BuildServiceProvider();
            Factory = Provider.GetService<IMessengerFactory>();

            GetConnected();
            RunConnected();
        }

        static string linePat = @"(?<command>[supe])\w*\s*(?<topic>\w+)?\s*(?<content>.*$)?";
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
                    this.Messenger = this.Factory.ConnectAsync(endpoint, CancellationToken.None).Result;
                }
                catch (Exception e)
                {
                    Console.WriteLine("?connect failed: " + e.Message);
                    continue;
                }

                if (Messenger != null)
                    break;

                Console.WriteLine("?failed to connect to: " + endpoint);
            }

            Messenger.OnMessage += Messenger_OnMessage;
        }

        private Task Messenger_OnMessage(IMessenger messenger, Message m)
        {
            
        }

        private Task Session_OnReceive(IMessenger session, ArraySegment<byte> m)
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
                            var bytes = UTF8Encoding.UTF8.GetBytes(m.Groups["topic"].Value);
                            var seg = new ArraySegment<byte>(bytes);
                            this.Messenger.SendAsync(seg);
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
