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
using Microsoft.AspNet.Hosting;

namespace WebSocketsDemo.Controllers
{
    internal class SessionWaiter : IActionResult
    {
        IPublisherSession session;

        public SessionWaiter(IPublisherSession session)
        {
            this.session = session;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            await session.WaitAsync(CancellationToken.None);
        }
    }

    [Route("[controller]")]
    public class SessionController : Controller
    {
        IPublisherSession session;

        public SessionController(IPublisherSession session)
        {
            this.session = session;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return new SessionWaiter(this.session);
        }
    }
}
