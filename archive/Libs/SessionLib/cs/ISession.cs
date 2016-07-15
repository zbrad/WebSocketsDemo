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
        event SessionHandler OnReceive;
        string Id { get; }
        bool IsClosed { get; }
        string CloseStatus { get; }
        string CloseDescription { get; }
        Task WaitAsync();
        Task SendAsync(ArraySegment<byte> m);
        Task SendAsync(ArraySegment<byte> m, CancellationToken token);
        Task CloseAsync();
    }

    public interface IServerSession : ISession
    {
    }

    public interface IClientSession : ISession
    {
        string Endpoint { get; }
    }
}
