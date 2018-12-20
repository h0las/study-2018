namespace TelegramNews.Database.Entities
{
    public class Post
    {
        public int Id { get; set; }

        public int ChannelId { get; set; }

        public string Content { get; set; }

        public int Views { get; set; }

        public int Type { get; set; }
    }
}