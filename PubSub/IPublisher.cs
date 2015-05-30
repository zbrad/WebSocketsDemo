using System.Collections.Generic;
using System.Threading.Tasks;
using Sessions;
using H = Microsoft.AspNet.Http;
using Microsoft.AspNet.Hosting;
using System.Threading;

namespace PubSub
{
    public interface IPublisher
    {
        void Add(IServerSession session);
    }

    public interface IPublisherSession
    {
        Task WaitAsync(CancellationToken token);
    }
}