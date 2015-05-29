using System.Collections.Generic;
using System.Threading.Tasks;
using Sessions;
using H = Microsoft.AspNet.Http;

namespace PubSub
{
    public interface IPublisher
    {
        Task<ISession> CreateSessionAsync(H.HttpContext context);
    }
}