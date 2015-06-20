using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using System.Net.WebSockets;
using System;
using System.Text;
using PubSubLib;

namespace PubSubServer.Controllers
{
    class SubscriberWaiter : IActionResult
    {
        Func<Task> func;

        public SubscriberWaiter(Func<Task> func)
        {
            this.func = func;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            await func();
        }
    }

    [Route("ws")]
    public class SessionController : Controller
    {
        IServerSubscriber subscriber;

        public SessionController(IServerSubscriber subscriber)
        {
            this.subscriber = subscriber;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return new SubscriberWaiter(Wait);
        }

        private async Task Wait()
        {
            await subscriber.WaitAsync();
        }
    }

}
