using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Framework.ConfigurationModel;
using Sessions.Messages;
using System.Collections.Generic;
using H = Microsoft.AspNet.Http;

namespace Sessions
{
    public interface ISessionFactory
    {
        int BufferSize { get; set; }
        Exception LastException { get; }
        ISession GetSession(string id);
        Task SendAsync(Message m, List<ISession> list);
    }
}