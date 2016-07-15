using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using System;
using Microsoft.AspNet.WebSockets.Server;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Messages;
using System.Threading;

namespace Sessions
{
    internal sealed class ServerSession : Session, IServerSession
    {
        //public ServerSession(ISessionFactory factory, IHttpWebSocketFeature feature) : base(factory)
        //{
        //    this.Socket = GetSocket(feature).Result;
        //}

        //async Task<WebSocket> GetSocket(IHttpWebSocketFeature feature)
        //{
        //    var accept = new WebSocketAcceptContext { ReceiveBufferSize = this.Factory.BufferSize };
        //    var socket = await feature.AcceptAsync(accept);
        //    if (socket == null)
        //        throw new InvalidOperationException("socket cannot be null");
        //    return socket;
        //}


        public ServerSession(ISessionFactory factory, IHttpContextAccessor accessor) : base(factory)
        {
            this.Socket = GetSocket(accessor).Result;
        }

        async Task<WebSocket> GetSocket(IHttpContextAccessor accessor)
        {
            var context = accessor.HttpContext;
            if (!context.IsWebSocketRequest)
                throw new InvalidOperationException("request is not a websocket request");

            var feature = context.GetFeature<IHttpWebSocketFeature>();
            var accept = new WebSocketAcceptContext { ReceiveBufferSize = this.Factory.BufferSize };
            var socket = await feature.AcceptAsync(accept);
            if (socket == null)
                throw new InvalidOperationException("socket cannot be null");

            return socket;
        }
    }
}