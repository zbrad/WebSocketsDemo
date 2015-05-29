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
        ConcurrentDictionary<string, Session> sessions = new ConcurrentDictionary<string, Session>();

        public const int DefaultBufferSize = 2048;
        public int BufferSize { get; set; }
        public Exception LastException { get; private set; }

        CancellationTokenSource cts = new CancellationTokenSource();

        public SessionFactory()
        {
            this.BufferSize = DefaultBufferSize;
        }

        internal void Add(Session session)
        {
            sessions[session.Id] = session;
        }

        internal void Remove(Session session)
        {
            sessions.TryRemove(session.Id, out session);
        }

        public async Task<ISession> CreateSessionAsync(H.HttpContext context)
        {
            var feature = context.GetFeature<H.IHttpWebSocketFeature>();
            if (feature == null)
                return null;

            var socket = await feature.AcceptAsync(new WebSocketAcceptContext { ReceiveBufferSize = this.BufferSize });
            return new ServerSession(this, socket);
        }

        public ISession GetSession(string id)
        {
            Session s;
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