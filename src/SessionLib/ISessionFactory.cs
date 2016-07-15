using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Framework.Configuration;
using System.Collections.Generic;
using H = Microsoft.AspNetCore.Http;

namespace SessionLib
{
    public interface ISessionFactory
    {
        int BufferSize { get; set; }
        Exception LastException { get; }
        Task SendAsync(ArraySegment<byte> m, List<ISession> list);
        Task<IClientSession> ConnectAsync(string endpoint);
        Task<IClientSession> ConnectAsync(string endpoint, CancellationToken token);
    }
}