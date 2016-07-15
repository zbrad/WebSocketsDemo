using Microsoft.AspNetCore.Http;
using System;
using Microsoft.AspNetCore.WebSockets.Server;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Threading;

namespace SessionLib
{
    internal sealed class ServerSession : Session, IServerSession
    {
        HttpContext context;

        public ServerSession(ISessionFactory factory, IHttpContextAccessor accessor) : base(factory)
        {
            this.context = accessor.HttpContext;
            if (!context.WebSockets.IsWebSocketRequest)
                throw new InvalidOperationException("request is not a websocket request");
            this.Socket = context.WebSockets.AcceptWebSocketAsync().Result;
        }
    }
}