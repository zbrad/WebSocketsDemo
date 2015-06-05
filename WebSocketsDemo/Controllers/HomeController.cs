using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

namespace WebSocketsDemo.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {

        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("[action]")]
        public IActionResult OldIndex()
        {
            return View();
        }

        [Route("[action]")]
        public IActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [Route("[action]")]
        public IActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [Route("[action]")]
        public IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml");
        }
    }
}
