using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SessionLib;

namespace MessageLib
{
    internal class MessengerFactory : IMessengerFactory
    {
        public Exception LastException { get; private set; }

        CancellationTokenSource cts = new CancellationTokenSource();

        public ISessionFactory Factory { get; private set; }

        public MessengerFactory(ISessionFactory factory)
        {
            this.Factory = factory;
        }

        public Task<IClientMessenger> ConnectAsync(string endpoint)
        {
            return this.ConnectAsync(endpoint, CancellationToken.None);
        }

        public async Task<IClientMessenger> ConnectAsync(string endpoint, CancellationToken token)
        {
            var session = await this.Factory.ConnectAsync(endpoint, token);
            if (session == null)
                return null;
            return new ClientMessenger(this, session);
        }

        public async Task SendAsync(Message m, List<IMessenger> list)
        {
            var bytes = m.Serialize();
            var seg = new ArraySegment<byte>(bytes);

            foreach (var s in list)
            {
                var messenger = (Messenger)s;
                await messenger.Session.SendAsync(seg);
            }
        }
    }
}