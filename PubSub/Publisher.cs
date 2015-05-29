using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sessions;
using System.Collections.Concurrent;
using Nito.AsyncEx;
using Sessions.Messages;
using System.Threading;
using H = Microsoft.AspNet.Http;

namespace PubSub
{
    internal class Publisher : IPublisher
    {
        ConcurrentDictionary<ISession, List<string>> perSessionSubs = new ConcurrentDictionary<ISession, List<string>>();
        ConcurrentDictionary<string, HashSet<ISession>> perTopicSubs = new ConcurrentDictionary<string, HashSet<ISession>>();
        AsyncCollection<TopicUpdate> queue = new AsyncCollection<TopicUpdate>();

        Task senderTask;
        CancellationTokenSource cts = new CancellationTokenSource();

        ISessionFactory factory;

        public Publisher(ISessionFactory factory)
        {
            this.factory = factory;
            senderTask = Task.Run(Sender);
        }

        public async Task<ISession> CreateSessionAsync(H.HttpContext context)
        {
            var session = await factory.CreateSessionAsync(context);
            session.ReceiveEvent += OnMessage;
            return session;
        }

        void OnMessage(ISession session, Message m)
        {
            if (m is Subscribe)
                AddSubscription(session, (Subscribe)m);
            else if (m is GetSubscriptions)
                GetSubscriptions(session);
            else if (m is TopicUpdate)
                Task.Run(async () => await Update(session, (TopicUpdate)m));
            else
            {
                var e = new Error("no matching message action for: " + m.GetType().Name);
                session.SendAsync(e);
            }
        }

        async Task Update(ISession session, TopicUpdate t)
        {
            t.Date = DateTime.UtcNow;
            t.Publisher = session.Id;
            await this.queue.AddAsync(t);
        }

        void AddSubscription(ISession session, Subscribe sub)
        {
            HashSet<ISession> subs;
            if (!perTopicSubs.TryGetValue(sub.Topic, out subs))
                subs = perTopicSubs[sub.Topic] = new HashSet<ISession>();

            lock (subs)
            {
                subs.Add(session);
            }

            List<string> list;
            if (!perSessionSubs.TryGetValue(session, out list))
                list = perSessionSubs[session] = new List<string>();

            list.Add(sub.Topic);
        }

        List<string> GetSubscriptions(ISession session)
        {
            List<string> list;
            if (perSessionSubs.TryGetValue(session, out list))
                return list;
            return null;
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
            HashSet<ISession> subs;
            if (!this.perTopicSubs.TryGetValue(t.Topic, out subs))
                return;

            List<ISession> list;
            lock (subs)
            {
                if (subs.Count == 0)
                    return;
                list = new List<ISession>(subs);
            }

            await this.factory.SendAsync(t, list);
        }
    }
}
