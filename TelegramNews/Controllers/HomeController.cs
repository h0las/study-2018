using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TelegramNews.ViewModels;

namespace TelegramNews.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public ViewResult Index()
        {
            var model = new IndexModel { content = "Hello, User!" };

            return View(model);
        }
    }
}