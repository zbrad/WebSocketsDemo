using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using PubSubLib;
using PubSubLib.Messages;
using ClientUtil;

namespace SubBar1
{
    public class Program
    {

        public void Main(string[] args)
        {
            Client client = new Client();
            client.GetConnected().Wait();
            client.Subscriber.SubscribeAsync("bar").Wait();
            Console.WriteLine("Subscribed for bar");
            client.Subscriber.WaitAsync().Wait();
        }

    }
}
