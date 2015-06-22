using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using PubSubLib;
using PubSubLib.Messages;
using System.Threading;
using ClientUtil;
namespace PubFooBar
{
    public class Program
    {
        public void Main(string[] args)
        {
            Client client = new Client();
            client.GetConnected().Wait();
            PubRandom(client).Wait();
        }

        int sequence = 0;
        async Task PubRandom(Client client)
        {
            var rand = new Random();
            
            while (true)
            {
                var value = Interlocked.Increment(ref sequence);
                var text = "content " + value;
                switch (rand.Next(3))
                {
                    case 0:
                        {
                            await client.Subscriber.PublishAsync("foo", text);
                            Console.WriteLine("Published foo: " + text);
                            break;
                        }
                    case 1:
                        {
                            await client.Subscriber.PublishAsync("bar", text);
                            Console.WriteLine("Published bar: " + text);
                            break;
                        }
                    case 2:
                        {
                            await client.Subscriber.PublishAsync("foo", text);
                            await client.Subscriber.PublishAsync("bar", text);
                            Console.WriteLine("Published foo,bar: " + text);
                            break;
                        }
                }

                await Task.Delay(2000);
            }

        }

    }
}
