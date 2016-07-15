using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Messages;
using System.Threading;

namespace Sessions
{
    public delegate void SessionHandler(ISession session, Message m);

    public interface ISession : IDisposable
    {
        string Id { get; }
        event SessionHandler OnReceive;
        Task WaitAsync(CancellationToken token);
        Task SendAsync(Message m);
        bool IsClosed { get; }
        Task CloseAsync();
        string CloseStatus { get; }
        string CloseDescription { get; }
    }

    public interface IServerSession : ISession
    {
    }

    public interface IClientSession : ISession
    {
        string Endpoint { get; }
    }
}
