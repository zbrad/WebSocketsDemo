using System.Net.WebSockets;
using SessionLib;

namespace MessageLib
{
    internal sealed class ClientMessenger : Messenger, IClientMessenger
    {

        public string Endpoint { get { return ((IClientSession)this.Session).Endpoint; } }

        public ClientMessenger(IMessengerFactory factory, IClientSession session) : base(factory, session)
        {
        }
    }
}