using System.Threading;
using System.Threading.Tasks;
using Sessions;
using System;
using Microsoft.AspNet.Mvc;
using Messages;

namespace PubSub
{

    internal class PublisherClient : PublisherSession, IPublisherClient
    {
        public event PublishClientHandler OnPublish;

        public PublisherClient(IPublisher publisher, IClientSession session) : base(publisher, session)
        {
        }

        protected override void Session_OnReceive(ISession session, Message m)
        {
            var p = this.OnPublish;
            if (p != null)
                p(this, m);
        }
    }
}
