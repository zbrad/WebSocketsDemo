using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using System;
using Microsoft.AspNet.WebSockets.Server;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Threading;
using SessionLib;

namespace MessageLib
{
    internal sealed class ServerMessenger : Messenger, IServerMessenger
    {
        public ServerMessenger(IMessengerFactory factory, IServerSession session) : base(factory, session)
        {
        }
    }
}