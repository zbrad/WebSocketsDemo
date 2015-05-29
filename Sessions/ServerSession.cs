using Microsoft.AspNet.Http;
using System.Threading.Tasks;
using Microsoft.AspNet.WebSockets.Protocol;
using Microsoft.AspNet.WebSockets.Server;
using System;
using System.Net.WebSockets;

namespace Sessions
{
    internal sealed class ServerSession : Session, IServerSession
    {
        public ServerSession(ISessionFactory factory, WebSocket socket) : base(factory)
        {
            this.Socket = socket;
        }
    }
}