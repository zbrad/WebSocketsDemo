using System.Net.WebSockets;

namespace SessionLib
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