using System.Threading;
using System.Threading.Tasks;
using Sessions;
using System;
using Microsoft.AspNet.Mvc;
using Messages;

namespace PubSub
{

    internal class PublisherServer : PublisherSession, IPublisherServer
    {
        public event PublishServerHandler OnPublish;

        public PublisherServer(IPublisher publisher, IServerSession session) : base(publisher, session)
        {
            this.Session.OnReceive += Session_OnReceive;
        }

        protected override void Session_OnReceive(ISession session, Message m)
        {
            this.Publisher.OnMessage(session, m);
        }
    }
}
