namespace TelegramNews.Database.Entities
{
    using System.Collections.Generic;

    public class Channel
    {
        public int ChannelId { get; set; }

        public ICollection<Post> Posts { get; set; }
    }
}