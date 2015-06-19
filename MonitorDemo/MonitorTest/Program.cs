using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PubSub;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using System.Threading;

namespace MonitorTest
{
    public class Program
    {
        IConfiguration Config { get; set; }
        IServiceProvider Provider { get; set; }

        IPublisher Publisher { get; set; }
        IPublisherClient Session { get; set; }

        public void Main(string[] args)
        {
            Config = new Configuration();
            var services = new ServiceCollection();
            services.AddPublisher();
            Provider = services.BuildServiceProvider();
            Publisher = Provider.GetService<IPublisher>();

            GetConnected();
        }

        static string linePat = @"(?<command>[supe])\w+\s*(?<topic>\w+)?\s*(?<content>.*$)?";
        static System.Text.RegularExpressions.Regex lineRegex = new System.Text.RegularExpressions.Regex(linePat);
        static string defaultEndpoint = "ws://localhost:44000/ws";

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
                    this.Session = Publisher.ConnectAsync(endpoint, CancellationToken.None).Result;
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

            Session.OnPublish += Session_OnPublish;
        }

        private void Session_OnPublish(IPublisherClient session, Messages.Message m)
        {
            Console.WriteLine("Received Message: " + m.Type);
            if (m is TopicUpdate)
            {
                var u = (TopicUpdate)m;
                Console.WriteLine("Topic: " + u.Topic + "\nContent: " + u.Content);
            }
            else
            {
                Console.WriteLine("Unhandled message type: " + m.GetType());
            }
        }

        void RunConnected()
        {
            while (true)
            {
                Console.WriteLine(@"
Options:
  [s]ubscribe topic
  [u]nsubcribe topic
  [p]ublish topic content
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
                            var topic = m.Groups["topic"].Value;
                            Session.SubscribeAsync(topic);
                            break;
                        }
                    case "u":
                        {
                            var topic = m.Groups["topic"].Value;
                            Session.UnsubscribeAsync(topic);
                            break;
                        }
                    case "p":
                        {
                            var topic = m.Groups["topic"].Value;
                            var content = m.Groups["content"].Value;
                            Session.PublishAsync(topic, content);
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
