using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.WebSockets.Client;
using System.Net.WebSockets;
using Messages;

namespace Sessions
{
    internal sealed class ClientSession : Session, IClientSession
    {

        public string Endpoint { get; private set; }

        public ClientSession(ISessionFactory factory, WebSocket ws, string endpoint) : base(factory)
        {
            this.Socket = ws;
            this.Endpoint = endpoint;            
        }
    }
}