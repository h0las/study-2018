namespace TelegramNews.Database.Entities
{
    public class Channel
    {
        public int Id { get; set; }
        public int LastMessageId { get; set; }
        public string ChannelName { get; set; }
    }
}