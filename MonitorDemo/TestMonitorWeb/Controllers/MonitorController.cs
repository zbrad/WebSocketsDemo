using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using PubSub;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using System.Net.WebSockets;
using System;

namespace TestMonitorWeb.Controllers
{
    class MonitorWaiter : IActionResult
    {
        Func<Task> func;

        public MonitorWaiter(Func<Task> func)
        {
            this.func = func;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            await func();
        }
    }

    [Route("ws")]
    public class MonitorController : Controller
    {
        IPublisherServer session;

        public MonitorController(IPublisherServer session)
        {
            this.session = session;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return new MonitorWaiter(Wait);
        }

        private async Task Wait()
        {
            var x = await session.WaitAsync();
        }
    }

}
