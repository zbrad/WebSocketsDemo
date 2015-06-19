using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using MessageLib;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using System.Net.WebSockets;
using System;
using System.Text;
using SharedMessages;

namespace MessageTest.Controllers
{
    class MessageWaiter : IActionResult
    {
        Func<Task> func;

        public MessageWaiter(Func<Task> func)
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
        IServerMessenger messenger;

        public SessionController(IServerMessenger messenger)
        {
            this.messenger = messenger;
            this.messenger.OnMessage += Messenger_OnMessage;
        }

        private Task Messenger_OnMessage(IMessenger messenger, Message m)
        {
            if (m is EchoRequest)
            {
                var request = (EchoRequest)m;
                var response = new EchoResponse("Echo: " + request.Text);
                messenger.SendAsync(response);
            }
            else
            {
                var error = new Error("unknown message: " + m.GetType());
                messenger.SendAsync(error);
            }

            return Task.FromResult<bool>(true);
        }

        [HttpGet]
        public IActionResult Get()
        {
            return new MessageWaiter(Wait);
        }

        private async Task Wait()
        {
            await messenger.WaitAsync();
        }
    }

}
