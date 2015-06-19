using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace MessageLib
{
    public delegate Task MessageHandler(IMessenger messenger, Message m);

    public interface IMessenger : IDisposable
    {
        event MessageHandler OnMessage;
        Task WaitAsync();
        Task SendAsync(Message m);
        Task SendAsync(Message m, CancellationToken token);
        bool IsClosed { get; }
        Task CloseAsync();
    }

    public interface IServerMessenger : IMessenger
    {
    }

    public interface IClientMessenger : IMessenger
    {
        string Endpoint { get; }
    }
}
