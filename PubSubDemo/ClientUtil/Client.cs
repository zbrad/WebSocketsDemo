using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using PubSubLib;
using PubSubLib.Messages;

namespace ClientUtil
{
    public sealed class Client
    {
        public IConfiguration Config { get; private set; }
        public IServiceProvider Provider { get; private set; }

        public IPublisherFactory Factory { get; private set; }
        public IClientSubscriber Subscriber { get; private set; }

        public Client()
        {
            Config = new Configuration();
            var services = new ServiceCollection();
            services.AddPublishers();
            Provider = services.BuildServiceProvider();
            Factory = Provider.GetService<IPublisherFactory>();
        }


        public async Task GetConnected()
        {
            while ((this.Subscriber = await Factory.ConnectAsync("ws://localhost:5401/ws"))
                == null)
            {
                Console.WriteLine("Waiting to connect...");
                await Task.Delay(5000);
            }

            Subscriber.OnPublication += Subscriber_OnPublication;
            Console.WriteLine("Connected");
        }


        private Task Subscriber_OnPublication(ISubscriber subscriber, MessageLib.Message m)
        {
            if (m is TopicUpdate)
            {
                var u = (TopicUpdate)m;
                Console.WriteLine(u.Date.ToString("yyyy-MM-ddTHH:mm:ssK") + "\tTopic: " + u.Topic + "\tContent: " + u.Content);
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
