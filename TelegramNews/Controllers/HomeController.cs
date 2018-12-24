using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TelegramNews.ViewModels;
using TelegramNews.Services;
using TelegramNews.Database.Entities;

namespace TelegramNews.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private IPostData _posts;
        private ITelegramServicesManager _telegramServicesManager;

        public HomeController(IPostData posts, ITelegramServicesManager telegramServicesManager)
        {
            _posts = posts;
            _telegramServicesManager = telegramServicesManager;
        }

        [AllowAnonymous]
        public ViewResult Index()
        {
            var model = new IndexModel { content = "Hello, User!" };

            return View(model);
        }

        public IActionResult ConnectToTelegram()
        {
            _telegramServicesManager.ConnectClientToTelegramAsync();

            if(_telegramServicesManager.IsUserAuthorized())
            {
                return RedirectToAction("LoadFeed", "Home");
            }
            else
            {
                return RedirectToAction("TelegramAuthorizeUser", "Home");
            }
        }

        [HttpGet]
        public IActionResult TelegramAuthorizeUser()
        {
            return View();
        }

        [HttpPost]
        public IActionResult TelegramAuthorizeUser(TelegramAuthorizeViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            return RedirectToAction("SendTelegramCode", "Home", new { phoneNumber = model.PhoneNumber});
        }

        public ViewResult LoadFeed()
        {
            var currentPosts = _telegramServicesManager.GetPosts(10, "myChannel").Result;
            var dbPosts = _posts.GetAll();

            var newPosts = currentPosts.Except(dbPosts, new PostComparer());

            _posts.Add(newPosts);

            var model = _posts.GetAll().Select(post =>
                new PostViewModel
                {
                    Content = post.Content,
                    Views = post.Views,
                    ChannelName = post.ChannelName
                });

            return View(model);
        }

        public IActionResult SendTelegramCode(string phoneNumber)
        {
            _telegramServicesManager.SendCodeRequestAsync(phoneNumber);

            return RedirectToAction("ConfirmTelegramCode", "Home");
        }

        [HttpGet]
        public IActionResult ConfirmTelegramCode()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmTelegramCode(TelegramCodeViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            await _telegramServicesManager.MakeAuthAsync(model.Code);

            if (_telegramServicesManager.IsUserAuthorized())
            {
                return RedirectToAction("LoadFeed", "Home");
            }

            ModelState.AddModelError("", "Authorized failded");
            return View(model);
        }
    }
}