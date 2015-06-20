using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Nito.AsyncEx;
using MessageLib;
using System.Threading;

namespace PubSubLib
{
    internal class PublisherFactory : IPublisherFactory
    {
        ConcurrentDictionary<IMessenger, List<string>> perMessengerSubs = new ConcurrentDictionary<IMessenger, List<string>>();
        ConcurrentDictionary<string, HashSet<IMessenger>> perTopicSubs = new ConcurrentDictionary<string, HashSet<IMessenger>>();
        AsyncCollection<TopicUpdate> queue = new AsyncCollection<TopicUpdate>();

        Task senderTask;
        CancellationTokenSource cts = new CancellationTokenSource();

        IMessengerFactory factory;

        public PublisherFactory(IMessengerFactory factory)
        {
            this.factory = factory;
            senderTask = Task.Run(Sender);
        }

        public async Task<IClientSubscriber> ConnectAsync(string endpoint, CancellationToken token)
        {
            var messenger = await this.factory.ConnectAsync(endpoint, token);
            if (messenger == null)
                return null;

            var pub = new ClientSubscriber(this, messenger);
            return pub;
        }

        public async Task OnMessageAsync(IMessenger messenger, Message m)
        {
            if (m is Subscribe)
                AddSubscription(messenger, (Subscribe)m);
            else if (m is Unsubscribe)
                RemoveSubscription(messenger, (Unsubscribe)m);
            else if (m is GetSubscriptions)
                await GetSubscriptions(messenger);
            else if (m is GetTopics)
                await GetTopics(messenger);
            else if (m is TopicUpdate)
                await Update(messenger, (TopicUpdate)m);
            else
            {
                var e = new Error("no matching message action for: " + m.GetType().Name);
                await messenger.SendAsync(e);
            }
        }

        async Task Update(IMessenger messenger, TopicUpdate t)
        {
            t.Date = DateTime.UtcNow;
            t.Publisher = messenger.Id;
            await this.queue.AddAsync(t);
        }

        void AddSubscription(IMessenger messenger, Subscribe sub)
        {
            HashSet<IMessenger> subs;
            if (!perTopicSubs.TryGetValue(sub.Topic, out subs))
                subs = perTopicSubs[sub.Topic] = new HashSet<IMessenger>();

            lock (subs)
            {
                subs.Add(messenger);
            }

            List<string> list;
            if (!perMessengerSubs.TryGetValue(messenger, out list))
                list = perMessengerSubs[messenger] = new List<string>();

            lock (list)
                list.Add(sub.Topic);
        }

        void RemoveSubscription(IMessenger messenger, Unsubscribe sub)
        {
            HashSet<IMessenger> subs;
            if (!perTopicSubs.TryGetValue(sub.Topic, out subs))
                subs = perTopicSubs[sub.Topic] = new HashSet<IMessenger>();

            lock (subs)
            {
                subs.Remove(messenger);
            }

            List<string> list;
            if (!perMessengerSubs.TryGetValue(messenger, out list))
                list = perMessengerSubs[messenger] = new List<string>();

            lock (list)
                list.Remove(sub.Topic);
        }

        async Task GetSubscriptions(IMessenger messenger)
        {
            List<string> list;
            if (!perMessengerSubs.TryGetValue(messenger, out list))
                list = new List<string>();
            var response = new Subscriptions(list);
            await messenger.SendAsync(response);
        }

        async Task GetTopics(IMessenger messenger)
        {
            var list = perTopicSubs.Keys.ToList();
            var response = new Topics(list);
            await messenger.SendAsync(response);
        }

        async Task Sender()
        {
            while (!cts.IsCancellationRequested)
            {
                var u = await this.queue.TakeAsync(cts.Token);
                if (u == null)
                    return;
                await SendSubs(u);
            }
        }

        async Task SendSubs(TopicUpdate t)
        {
            HashSet<IMessenger> subs;
            if (!this.perTopicSubs.TryGetValue(t.Topic, out subs))
                return;

            List<IMessenger> list;
            lock (subs)
            {
                if (subs.Count == 0)
                    return;
                list = new List<IMessenger>(subs);
            }

            await this.factory.SendAsync(t, list);
        }
    }
}
