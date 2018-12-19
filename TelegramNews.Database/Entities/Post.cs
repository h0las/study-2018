namespace TelegramNews.Database.Entities
{
    public class Post
    {
        public int PostId { get; set; }

        public string Content { get; set; }

        public int ChannelId { get; set; }
    }
}