using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using System;
using Microsoft.AspNet.WebSockets.Server;
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
            if (!context.IsWebSocketRequest)
                throw new InvalidOperationException("request is not a websocket request");
            this.Socket = context.AcceptWebSocketAsync().Result;
        }
    }
}