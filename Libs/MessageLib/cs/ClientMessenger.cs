using System.Net.WebSockets;
using SessionLib;

namespace MessageLib
{
    internal sealed class ClientMessenger : Messenger, IClientMessenger
    {

        public string Endpoint { get; private set; }

        public ClientMessenger(IMessengerFactory factory, IClientSession session, string endpoint) : base(factory, session)
        {
            this.Endpoint = endpoint;            
        }
    }
}