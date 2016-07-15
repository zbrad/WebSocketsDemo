using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using PubSubLib;
using PubSubLib.Messages;
using ClientUtil;
namespace SubFooBar
{
    public class Program
    {
        public void Main(string[] args)
        {
            Client client = new Client();
            client.GetConnected().Wait();
            client.Subscriber.SubscribeAsync("foo").Wait();
            client.Subscriber.SubscribeAsync("bar").Wait();
            Console.WriteLine("Subscribed for: foo,bar");
            client.Subscriber.WaitAsync().Wait();
        }
    }
}
