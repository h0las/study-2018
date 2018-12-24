namespace TelegramNews.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class PostViewModel
    {
        public string ChannelName { get; set; }
        public string Content { get; set; }
        public int? Views { get; set; }
    }
}