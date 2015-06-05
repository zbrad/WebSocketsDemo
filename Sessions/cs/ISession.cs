using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sessions.Messages;
using System.Threading;

namespace Sessions
{
    public delegate void MessageEventHandler(ISession session, Message m);

    public interface ISession : IDisposable
    {
        string Id { get; }
        Task WaitAsync(CancellationToken token);
        event MessageEventHandler ReceiveEvent;
        Task SendAsync(Message m);
        bool IsClosed { get; }
        Task CloseAsync();
        string CloseStatus { get; }
        string CloseDescription { get; }
    }

    public interface IServerSession : ISession { }

    public interface IClientSession : ISession
    {
        string Endpoint { get; }
        Task<bool> TryConnectAsync(string endpoint, CancellationToken token);
    }
}
