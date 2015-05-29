using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.WebSockets.Client;

namespace Sessions
{
    internal sealed class ClientSession : Session, IClientSession
    {

        public ClientSession(ISessionFactory factory) : base(factory)
        {

        }

        public async Task<bool> TryConnectAsync(Uri u, CancellationToken token)
        {
            var client = new WebSocketClient()
            {
                ReceiveBufferSize = this.Factory.BufferSize
            };

            try
            {
                var ws = await client.ConnectAsync(u, token);
                this.Socket = ws;
                return true;
            }
            catch (Exception e)
            {
                this.LastException = e;
                return false;
            }
        }
    }
}