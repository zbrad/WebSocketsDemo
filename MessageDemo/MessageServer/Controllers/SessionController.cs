using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Sessions;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using System.Net.WebSockets;
using System;
using System.Text;

namespace TestMonitorWeb.Controllers
{
    class SessionWaiter : IActionResult
    {
        Func<Task> func;

        public SessionWaiter(Func<Task> func)
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
        IServerSession session;

        public SessionController(IServerSession session)
        {
            this.session = session;
            this.session.OnReceive += Session_OnReceive;
        }

        private async Task Session_OnReceive(Sessions.ISession session, ArraySegment<byte> m)
        {
            var message = UTF8Encoding.UTF8.GetString(m.Array, 0, m.Count);
            Console.WriteLine("Received message: " + message);
            var echo = "Echo: " + message;
            var bytes = UTF8Encoding.UTF8.GetBytes(echo);
            var seg = new ArraySegment<byte>(bytes);
            await session.SendAsync(seg);
        }

        [HttpGet]
        public IActionResult Get()
        {
            return new SessionWaiter(Wait);
        }

        private async Task Wait()
        {
            await session.WaitAsync();
        }
    }

}
