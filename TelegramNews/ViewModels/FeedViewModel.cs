namespace TelegramNews.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class FeedViewModel
    {
        public IEnumerable<PostViewModel> Posts { get; set; }
        public IEnumerable<ChannelViewModel> Channels { get; set; }
    }
}