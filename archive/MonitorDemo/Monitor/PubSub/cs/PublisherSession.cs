using System.Threading;
using System.Threading.Tasks;
using Sessions;
using System;
using Microsoft.AspNet.Mvc;
using Messages;

namespace PubSub
{

    internal abstract class PublisherSession : IPublisherSession
    {
        protected ISession Session { get; private set; }
        protected Publisher Publisher { get; private set; }

        protected abstract void Session_OnReceive(ISession session, Message m);

        public PublisherSession(IPublisher publisher, ISession session)
        {
            this.Publisher = (Publisher) publisher;
            this.Session = session;
            this.Session.OnReceive += Session_OnReceive;
        }

        public async Task<IActionResult> WaitAsync(CancellationToken token)
        {
            await this.Session.WaitAsync(token);
            return new HttpStatusCodeResult(200);
        }

        public Task<IActionResult> WaitAsync()
        {
            return this.WaitAsync(CancellationToken.None);
        }

        public async Task PublishAsync(string topic, string content)
        {
            var u = new TopicUpdate(topic, content);
            await this.Session.SendAsync(u);
        }        

        public async Task SubscribeAsync(string topic)
        {
            var s = new Subscribe(topic);
            await this.Session.SendAsync(s);
        }

        public async Task UnsubscribeAsync(string topic)
        {
            var u = new Unsubscribe(topic);
            await this.Session.SendAsync(u);
        }
    }
}
