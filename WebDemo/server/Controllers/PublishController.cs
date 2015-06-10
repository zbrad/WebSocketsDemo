using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Net.Http;
using PubSub;
using System.Threading;

namespace WebDemo.Controllers
{
    class Result : IActionResult
    {
        Func<Task> func;

        public Result(Func<Task> func)
        {
            this.func = func;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            await func();
        }
    }

    [Route("publish")]
    public class PublishController : Controller
    {
        
        public IActionResult Waiter(IPublisherSession pub)
        {
            return new Result(async () =>
            {
                await pub.WaitAsync();
            });
        }
    }
}
