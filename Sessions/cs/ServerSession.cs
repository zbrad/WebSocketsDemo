using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using System;
using Microsoft.AspNet.WebSockets.Server;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Sessions
{
    internal sealed class ServerSession : Session, IServerSession
    {
        public ServerSession(ISessionFactory factory, IHttpContextAccessor accessor) : base(factory)
        {
            this.Socket = GetSocket(accessor).Result;
        }

        async Task<WebSocket> GetSocket(IHttpContextAccessor accessor)
        {
            var feature = accessor.HttpContext.GetFeature<IHttpWebSocketFeature>();
            if (feature == null)
                throw new InvalidOperationException("requires an http context");

            var accept = new WebSocketAcceptContext { ReceiveBufferSize = this.Factory.BufferSize };
            var socket = await feature.AcceptAsync(accept);
            if (socket == null)
                throw new InvalidOperationException("socket cannot be null");

            return socket;
        }
    }
}