using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Sessions;
using PubSub;
// add package: "Microsoft.AspNet.WebApi.Core"
//using System.Web.Http;
using System.Net.Http;
using Sessions.Messages;
using Microsoft.AspNet.Mvc;

namespace WebSocketsDemo.Controllers
{
    internal class SessionWaiter : IActionResult
    {
        IPublisher publisher;

        public SessionWaiter(IPublisher publisher)
        {
            this.publisher = publisher;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            if (publisher == null)
                return;

            var session = await publisher.CreateSessionAsync(context.HttpContext);
            await session.WaitAsync(CancellationToken.None);
        }
    }

    [Route("[controller]")]
    public class SessionController : Controller
    {
        IPublisher publisher;
        public SessionController(IPublisher publisher)
        {
            this.publisher = publisher;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return new SessionWaiter(this.publisher);
        }
    }
}
