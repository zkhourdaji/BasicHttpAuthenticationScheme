using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AuthenticationSchemeDemo.Models;
using Microsoft.AspNetCore.Authorization;

namespace AuthenticationSchemeDemo.Controllers
{
    [Authorize(AuthenticationSchemes = "Basic")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return Content("Access granted");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
