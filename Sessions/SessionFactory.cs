using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Collections.Generic;
using Microsoft.Framework.ConfigurationModel;
using Nito.AsyncEx;
using System.Threading;
using System.Threading.Tasks;
using Sessions.Messages;
using H = Microsoft.AspNet.Http;
using Microsoft.AspNet.WebSockets.Server;

namespace Sessions
{
    internal class SessionFactory : ISessionFactory
    {
        ConcurrentDictionary<string, ISession> sessions = new ConcurrentDictionary<string, ISession>();

        public const int DefaultBufferSize = 2048;
        public int BufferSize { get; set; }
        public Exception LastException { get; private set; }

        CancellationTokenSource cts = new CancellationTokenSource();

        public SessionFactory()
        {
            this.BufferSize = DefaultBufferSize;
        }

        internal void Add(ISession session)
        {
            sessions[session.Id] = session;
        }

        internal void Remove(ISession session)
        {
            sessions.TryRemove(session.Id, out session);
        }

        public ISession GetSession(string id)
        {
            ISession s;
            if (sessions.TryGetValue(id, out s))
                return s;
            return null;
        }

        public async Task SendAsync(Message m, List<ISession> list)
        {
            var seg = new ArraySegment<byte>(m.Serialize());
            foreach (var s in list)
            {
                Session session = (Session)s;
                await session.SendAsync(seg);
            }
        }
    }
}