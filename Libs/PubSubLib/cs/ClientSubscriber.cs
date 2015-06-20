using System.Threading;
using System.Threading.Tasks;
using System;
using Microsoft.AspNet.Mvc;
using MessageLib;

namespace PubSubLib
{

    internal class ClientSubscriber : Subscriber, IClientSubscriber
    {
        public event PublicationHandler OnPublication;

        public string Endpoint { get { return ((IClientMessenger)this.Messenger).Endpoint; } }

        public ClientSubscriber(IPublisherFactory publisher, IClientMessenger session) : base(publisher, session)
        {
        }

        protected override async Task Messenger_OnMessage(IMessenger messenger, Message m)
        {
            var p = this.OnPublication;
            if (p != null)
                await p(this, m);
        }
    }
}
