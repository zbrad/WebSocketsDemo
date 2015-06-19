using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Collections.Generic;
using Microsoft.Framework.ConfigurationModel;
using Nito.AsyncEx;
using System.Threading;
using System.Threading.Tasks;
using H = Microsoft.AspNet.Http;
using Microsoft.AspNet.WebSockets.Server;
using Microsoft.AspNet.WebSockets.Client;

namespace SessionLib
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

        public async Task<IClientSession> ConnectAsync(string endpoint, CancellationToken token)
        {
            var wsc = new WebSocketClient()
            {
                ReceiveBufferSize = this.BufferSize
            };

            try
            {
                var ws = await wsc.ConnectAsync(new Uri(endpoint), token);
                var client = new ClientSession(this, ws, endpoint);
                return client;
            }
            catch (Exception e)
            {
                this.LastException = e;
                return null;
            }
        }

        public void Add(ISession session)
        {
            sessions[session.Id] = session;
        }

        public void Remove(ISession session)
        {
            sessions.TryRemove(session.Id, out session);
        }

        public async Task SendAsync(ArraySegment<byte> m, List<ISession> list)
        {
            foreach (var s in list)
            {
                Session session = (Session)s;
                await session.SendAsync(m);
            }
        }
    }
}