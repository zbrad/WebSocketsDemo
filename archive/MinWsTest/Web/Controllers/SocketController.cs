using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using System.Net.WebSockets;
using System;
using System.Threading;

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
        HttpContext context;
        WebSocket socket;

        public MonitorController(IHttpContextAccessor accessor)
        {
            this.context = accessor.HttpContext;
            if (!this.context.IsWebSocketRequest)
                throw new InvalidOperationException("incoming request is not a websocket request");
        }

        [HttpGet]
        public IActionResult Get()
        {
            return new MonitorWaiter(Wait);
        }

        private async Task Wait()
        {
            byte[] buffer = new byte[4096];
            var seg = new ArraySegment<byte>(buffer);

            try
            {
                socket = await this.context.AcceptWebSocketAsync();
                while (socket != null && socket.State == WebSocketState.Open)
                {
                    var result = await socket.ReceiveAsync(seg, CancellationToken.None);
                    if (result.CloseStatus.HasValue)
                    {
                        Console.WriteLine("socket was closed: " + result.CloseStatusDescription);
                        break;
                    }

                    var sendSeg = new ArraySegment<byte>(buffer, 0, result.Count);
                    await socket.SendAsync(sendSeg, WebSocketMessageType.Binary, true, CancellationToken.None);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("websocket exception: " + e.Message);
            }

            socket.Dispose();
            socket = null;
        }
    }

}
