using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.WebSockets.Client;

namespace Sessions
{
    internal sealed class ClientSession : Session, IClientSession
    {
        public string Endpoint { get; private set; }

        public ClientSession(ISessionFactory factory) : base(factory)
        {

        }

        public async Task<bool> TryConnectAsync(string endpoint, CancellationToken token)
        {
            var client = new WebSocketClient()
            {
                ReceiveBufferSize = this.Factory.BufferSize
            };

            try
            {
                var ws = await client.ConnectAsync(new Uri(endpoint), token);
                this.Socket = ws;
                this.Endpoint = endpoint;
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