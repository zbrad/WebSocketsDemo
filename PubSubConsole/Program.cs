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
using Nito.AsyncEx;

namespace PubSubConsole
{
    public class Program
    {
        IServiceProvider provider;
        AsyncAutoResetEvent waitForMessage = new AsyncAutoResetEvent();

        public void Main(string[] args)
        {
            ServiceCollection services = new ServiceCollection();
            services.AddSessions();
            provider = services.BuildServiceProvider();
            RunPub().Wait();
            Console.WriteLine("[Press enter to exit]");
            Console.ReadLine();
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
                await waitForMessage.WaitAsync();

                // test that we got a subscriptions message back (or other)
                if (lastMessage == null)
                {
                    Console.WriteLine("received message signaled, but lastMessage is null");
                    return;
                }

                if (lastMessage is Subscriptions)
                {
                    var subs = (Subscriptions)lastMessage;
                    if (subs.List.Count == 0)
                        Console.WriteLine("Current subscription list is empty");
                    else
                    {
                        Console.WriteLine("Current subscriptions are:");
                        foreach (var s in subs.List)
                            Console.WriteLine(s);
                    }
                }
                else
                {
                    Console.WriteLine("Received unexpected message: " + lastMessage.GetType().Name);
                }
            }
        }

        Message lastMessage;

        private void Session_ReceiveEvent(ISession session, Message m)
        {
            lastMessage = m;
            waitForMessage.Set();
        }
    }
}
