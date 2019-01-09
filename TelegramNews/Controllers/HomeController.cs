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
            return View();
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

        public ViewResult LoadFeed(string channelFilter = "")
        {
            IEnumerable<Channel> channels;
            IEnumerable<Post> posts;

            channels = channelFilter == string.Empty ? 
                channels = _posts.GetAllChannels().ToList() :
                _posts.GetAllChannels().Where(channel => channel.ChannelName == channelFilter).ToList();

            var channelNames = channels.Select(channel => channel.ChannelName).ToArray();
            
            var currentPosts = new List<Post>();

            foreach(var channel in channels)
            {
                var currentChannel = _posts.GetChannel(channel.Id);
                
                var channelPosts = _telegramServicesManager.GetPosts(24, currentChannel.LastMessageId, channel.ChannelName).Result.ToList();

                if (channelPosts.Count != 0)
                {
                    var lastChannelMessage = channelPosts.Last();
                    currentChannel.LastMessageId = lastChannelMessage.TgMessageId;
                    _posts.Commit();
                }

                currentPosts = currentPosts.Union(channelPosts, new PostComparer()).ToList();
            }

            var dbPosts = _posts.GetAll();

            var newPosts = currentPosts.Except(dbPosts, new PostComparer()).Reverse();

            _posts.Add(newPosts);

            var maxPreviewContentLength = 150;

            posts = channelFilter == string.Empty ? 
                _posts.GetAll().Where(post => channelNames.Contains(post.ChannelName)).TakeLast(12) : 
                _posts.GetAll().Where(post => channelNames.Contains(post.ChannelName)).Take(24);

            var model = new FeedViewModel
            {
                Posts = posts.Select(post =>
                new PostViewModel
                {
                    PreviewContent = post.Content.Length > maxPreviewContentLength ?
                        post.Content.Substring(0, Math.Min(post.Content.Length, maxPreviewContentLength)) + @"..." :
                        post.Content.Substring(0, Math.Min(post.Content.Length, maxPreviewContentLength)),
                    Views = post.Views,
                    ChannelName = post.ChannelName,
                    Id = post.Id,
                    Type = post.Type,
                    Title = post.Title,
                    Url = post.Url,
                    FileType = post.FileType
                }),
                Channels = channels.Select(channel => new ChannelViewModel { ChannelName = channel.ChannelName })
            };  

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
                _posts.Add(new Channel { ChannelName = model.ChannelName, LastMessageId = -1 });
                return RedirectToAction("LoadFeed", "Home");
            }

            ModelState.AddModelError("", "Authorized failded");
            return View(model);
        }

        public IActionResult Details(int id)
        {
            var model = _posts.Get(id);
            var viewModel = new PostViewModel
            {
                Id = model.Id,
                Content = model.Content,
                Views = model.Views,
                ChannelName = model.ChannelName,
                Title = model.Title,
                Url = model.Url,
                Type = model.Type
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult ChannelsList()
        {
            return View(_posts.GetAllChannels());
        }
    }
}