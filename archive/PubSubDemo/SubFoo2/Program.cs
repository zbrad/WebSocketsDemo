using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using PubSubLib;
using PubSubLib.Messages;

namespace SubFoo1
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
            SubscribeFoo().Wait();
            this.Subscriber.WaitAsync().Wait();
        }

        static string defaultEndpoint = "ws://localhost:57762/ws";

        async Task GetConnected()
        {
            this.Subscriber = await Factory.ConnectAsync(defaultEndpoint);
            Subscriber.OnPublication += Subscriber_OnPublication;
        }

        async Task SubscribeFoo()
        {
            await this.Subscriber.SubscribeAsync("foo");
        }

        private Task Subscriber_OnPublication(ISubscriber subscriber, MessageLib.Message m)
        {
            if (m is TopicUpdate)
            {
                var u = (TopicUpdate)m;
                Console.WriteLine("\tUpdate:\tDate: " + u.Date + "\tTopic: " + u.Topic + "\tContent: " + u.Content);
            }
            else if (m is Subscriptions)
            {
                Console.WriteLine("\tSubscriptions:");
                foreach (var s in ((Subscriptions)m).List)
                    Console.WriteLine("\t\t" + s);
            }
            else if (m is Topics)
            {
                Console.WriteLine("\tTopics:");
                foreach (var s in ((Topics)m).List)
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
    }
}
