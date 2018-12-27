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
using Microsoft.Extensions.Configuration;
using System.IO;

namespace TelegramNews.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private IPostData _posts;
        private ITelegramServicesManager _telegramServicesManager;
        private IConfiguration _config;

        public HomeController(IPostData posts, ITelegramServicesManager telegramServicesManager, IConfiguration configuration)
        {
            _posts = posts;
            _telegramServicesManager = telegramServicesManager;
            _config = configuration;
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
            var channels = _posts.GetAllChannels().ToList();
            var currentPosts = new List<Post>();

            foreach(var channel in channels)
            {
                var channelPosts = _telegramServicesManager.GetPosts(5, channel.ChannelName).Result.ToList();
                currentPosts = currentPosts.Union(channelPosts, new PostComparer()).ToList();
            }

            var dbPosts = _posts.GetAll();

            var newPosts = currentPosts.Except(dbPosts, new PostComparer());

            _posts.Add(newPosts);

            var model = _posts.GetAll().Select(post =>
                new PostViewModel
                {
                    Content = post.Content,
                    Views = post.Views,
                    ChannelName = post.ChannelName,
                    Id = post.Id,
                    Type = post.Type,
                    Title = post.Title,
                    Url = post.Url,
                    FileType = post.FileType
                });

            return View(model);
        }

        public FileContentResult GetFile(int messageId)
        {
            var post = _posts.Get(messageId);

            return File(post.File, post.FileType);
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
                return RedirectToAction("AddTelegramChannel", "Home");
            }

            ModelState.AddModelError("", "Authorized failded");
            return View(model);
        }

        [HttpGet]
        public IActionResult AddTelegramChannel()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddTelegramChannel(TelegramChannelViewModel model)
        {
            if(model.ChannelName != string.Empty)
            {
                _posts.Add(new Channel { ChannelName = model.ChannelName });
                return RedirectToAction("LoadFeed", "Home");
            }

            ModelState.AddModelError("", "Authorized failded");
            return View(model);
        }
    }
}