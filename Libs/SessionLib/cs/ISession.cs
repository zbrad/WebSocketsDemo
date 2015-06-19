using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace SessionLib
{
    public delegate Task SessionHandler(ISession session, ArraySegment<byte> m);

    public interface ISession : IDisposable
    {
        string Id { get; }
        event SessionHandler OnReceive;
        Task WaitAsync();
        Task SendAsync(ArraySegment<byte> m);
        Task SendAsync(ArraySegment<byte> m, CancellationToken token);
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
