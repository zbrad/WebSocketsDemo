using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Framework.ConfigurationModel;
using System.Collections.Generic;
using H = Microsoft.AspNet.Http;

namespace MessageLib
{
    public interface IMessengerFactory
    {
        Exception LastException { get; }
        Task SendAsync(Message m, List<IMessenger> list);
        Task<IClientMessenger> ConnectAsync(string endpoint);

        Task<IClientMessenger> ConnectAsync(string endpoint, CancellationToken token);
    }
}