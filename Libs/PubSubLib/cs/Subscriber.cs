using System.Threading;
using System.Threading.Tasks;
using System;
using Microsoft.AspNet.Mvc;
using MessageLib;

namespace PubSubLib
{

    internal abstract class Subscriber : ISubscriber
    {
        protected IMessenger Messenger { get; private set; }
        protected PublisherFactory Factory { get; private set; }

        public Subscriber(IPublisherFactory publisher, IMessenger messenger)
        {
            this.Factory = (PublisherFactory) publisher;
            this.Messenger = messenger;
            this.Messenger.OnMessage += Messenger_OnMessage;
        }

        protected abstract Task Messenger_OnMessage(IMessenger messenger, Message m);

        public string Id { get { return this.Messenger.Id; } }

        public bool IsClosed { get { return this.Messenger.IsClosed; } }

        public Task WaitAsync()
        {
            return this.Messenger.WaitAsync();
        }

        public Task CloseAsync()
        {
            return this.Messenger.CloseAsync();
        }

        public async Task PublishAsync(string topic, string content)
        {
            var u = new TopicUpdate(topic, content);
            await this.Messenger.SendAsync(u);
        }        

        public async Task SubscribeAsync(string topic)
        {
            var s = new Subscribe(topic);
            await this.Messenger.SendAsync(s);
        }

        public async Task UnsubscribeAsync(string topic)
        {
            var u = new Unsubscribe(topic);
            await this.Messenger.SendAsync(u);
        }

        public async Task GetTopicsAsync()
        {
            var t = new Topics();
            await this.Messenger.SendAsync(t);
        }

        public async Task GetSubscriptionsAsync()
        {
            var s = new Subscriptions();
            await this.Messenger.SendAsync(s);
        }
    }
}
