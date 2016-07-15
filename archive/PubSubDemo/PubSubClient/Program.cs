using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PubSubLib;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using M = PubSubLib.Messages;

namespace PubSubClient
{
    public class Program
    {
        IConfiguration Config { get; set; }
        IServiceProvider Provider { get; set; }

        IPublisherFactory Factory { get; set; }
        IClientSubscriber Subscriber { get; set; }

        public void Main(string[] args)
        {
            Config = new Configuration();
            var services = new ServiceCollection();
            services.AddPublisher();
            Provider = services.BuildServiceProvider();
            Factory = Provider.GetService<IPublisherFactory>();

            GetConnected().Wait();
            RunConnected().Wait();
        }

        static string linePat = @"(?<command>[a-z])\w*\s*(?<topic>\w+)?\s*(?<content>.*$)?";
        static System.Text.RegularExpressions.Regex lineRegex = new System.Text.RegularExpressions.Regex(linePat);
        static string defaultEndpoint = "ws://localhost:57762/ws";

        async Task GetConnected()
        {
            while (true)
            {
                Console.Write("Endpoint[" + defaultEndpoint + "]: ");
                var endpoint = Console.ReadLine();
                if (string.IsNullOrEmpty(endpoint))
                    endpoint = defaultEndpoint;

                try
                {
                    this.Subscriber = await Factory.ConnectAsync(endpoint, CancellationToken.None);
                }
                catch (Exception e)
                {
                    Console.WriteLine("?connect failed: " + e.Message);
                    continue;
                }

                if (Subscriber != null)
                    break;

                Console.WriteLine("?failed to connect to: " + endpoint);
            }

            Subscriber.OnPublication += Subscriber_OnPublication;
        }

        private Task Subscriber_OnPublication(ISubscriber subscriber, MessageLib.Message m)
        {
            if (m is M.TopicUpdate)
            {
                var u = (M.TopicUpdate)m;
                Console.WriteLine("\tUpdate:\tDate: " + u.Date + "\tTopic: " + u.Topic + "\tContent: " + u.Content);
            }
            else if (m is M.Subscriptions)
            {
                Console.WriteLine("\tSubscriptions:");
                foreach (var s in ((M.Subscriptions)m).List)
                    Console.WriteLine("\t\t" + s);
            }
            else if (m is M.Topics)
            {
                Console.WriteLine("\tTopics:");
                foreach (var s in ((M.Topics)m).List)
                    Console.WriteLine("\t\t" + s);

            }
            else if (m is MessageLib.Error)
            {
                Console.WriteLine("\tError: " + ((MessageLib.Error)m).Message);
            }
            else
            {
                Console.WriteLine("\t?Unhandled message type: " + m.GetType());
            }

            return Task.FromResult<bool>(true);
        }

        async Task RunConnected()
        {
            while (this.Subscriber != null && !this.Subscriber.IsClosed)
            {
                Console.WriteLine(@"
Options:
  [s]ubscribe {topic}
  [u]nsubcribe {topic}
  [g]et-subscriptions
  [t]opics-list
  [p]ublish {topic} {content}
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
                            await Subscriber.SubscribeAsync(topic);
                            break;
                        }
                    case "u":
                        {
                            var topic = m.Groups["topic"].Value;
                            await Subscriber.UnsubscribeAsync(topic);
                            break;
                        }
                    case "p":
                        {
                            var topic = m.Groups["topic"].Value;
                            var content = m.Groups["content"].Value;
                            await Subscriber.PublishAsync(topic, content);
                            break;
                        }
                    case "t":
                        await Subscriber.GetTopicsAsync();
                        break;
                    case "g":
                        await Subscriber.GetSubscriptionsAsync();
                        break;
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
