namespace TelegramNews.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using TelegramNews.Database.Entities;

    public class PostViewModel
    {
        public string ChannelName { get; set; }
        public string Content { get; set; }
        public string PreviewContent { get; set; }
        public int? Views { get; set; }
        public int Id { get; set; }
        public EnChannelMessage Type { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string FileType { get; set; }
    }
}