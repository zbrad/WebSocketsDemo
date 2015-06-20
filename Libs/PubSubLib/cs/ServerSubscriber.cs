using System.Threading;
using System.Threading.Tasks;
using System;
using Microsoft.AspNet.Mvc;
using MessageLib;

namespace PubSubLib
{

    internal class ServerSubscriber : Subscriber, IServerSubscriber
    {
        public event PublicationHandler OnPublication;

        public ServerSubscriber(IPublisherFactory factory, IServerMessenger messenger) : base(factory, messenger)
        {
        }

        protected override Task Messenger_OnMessage(IMessenger messenger, Message m)
        {
            return this.Factory.OnMessageAsync(messenger, m);
        }
    }
}
