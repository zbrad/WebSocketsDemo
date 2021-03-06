﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sessions;
using System.Collections.Concurrent;
using Nito.AsyncEx;
using Messages;
using System.Threading;

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

        public async Task<IPublisherClient> ConnectAsync(string endpoint, CancellationToken token)
        {
            var session = await this.factory.ConnectAsync(endpoint, token);
            if (session == null)
                return null;

            var pub = new PublisherClient(this, session);
            return pub;
        }

        public void OnMessage(ISession session, Message m)
        {
            if (m is Subscribe)
                AddSubscription(session, (Subscribe)m);
            else if (m is Unsubscribe)
                RemoveSubscription(session, (Unsubscribe)m);
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

            lock (list)
                list.Add(sub.Topic);
        }

        void RemoveSubscription(ISession session, Unsubscribe sub)
        {
            HashSet<ISession> subs;
            if (!perTopicSubs.TryGetValue(sub.Topic, out subs))
                subs = perTopicSubs[sub.Topic] = new HashSet<ISession>();

            lock (subs)
            {
                subs.Remove(session);
            }

            List<string> list;
            if (!perSessionSubs.TryGetValue(session, out list))
                list = perSessionSubs[session] = new List<string>();

            lock (list)
                list.Remove(sub.Topic);
        }

        void GetSubscriptions(ISession session)
        {
            List<string> list;
            if (!perSessionSubs.TryGetValue(session, out list))
                list = new List<string>();
            var response = new Subscriptions(list);
            session.SendAsync(response);
        }

        void GetTopics(ISession session, string topic)
        {
            var list = perTopicSubs.Keys.ToList();
            var response = new Topics(list);
            session.SendAsync(response);
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
