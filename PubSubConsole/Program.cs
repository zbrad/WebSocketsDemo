using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sessions;
using Sessions.Messages;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.ConfigurationModel.Json;
using Microsoft.Framework.DependencyInjection;
using System.Threading;
using PubSub;

namespace PubSubConsole
{
    public class Program
    {
        IServiceProvider provider;

        public void Main(string[] args)
        {
            ServiceCollection services = new ServiceCollection();
            services.AddSessions();
            provider = services.BuildServiceProvider();

            RunPub().Wait();
        }

        async Task RunPub()
        {

            var session = provider.GetService<IClientSession>();
            session.ReceiveEvent += Session_ReceiveEvent;

            bool result = await session.TryConnectAsync(new Uri("ws://localhost:57985/session"), CancellationToken.None);
            if (result)
            {
                var m = new GetSubscriptions();
                await session.SendAsync(m);

                // now wait for a response
                while (!gotResponse)
                    await Task.Delay(1000);
            }
        }

        bool gotResponse = false;

        private void Session_ReceiveEvent(ISession session, Message m)
        {
            if (m is Subscriptions)
            {
                gotResponse = true;
            }
        }
    }
}
