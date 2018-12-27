namespace TelegramNews.Database.Entities
{
    using System.Collections.Generic;
    using System.Linq;

    public class Post
    {
        public int Id { get; set; }

        public int TgMessageId { get; set; }

        public int ChannelId { get; set; }

        public string ChannelName { get; set; }

        public string Content { get; set; }

        public int? Views { get; set; }

        public EnChannelMessage Type { get; set; }

        public byte[] File { get; set; }

        public string FileType { get; set; }

        public string Url { get; set; }

        public string Title { get; set; }
    }

    public enum EnChannelMessage
    {
        Message,
        MediaPhoto,
        MediaDocument,
        WebPage
    }

    public class PostComparer : IEqualityComparer<Post>
    {
        public bool Equals(Post x, Post y)
        {
            if((x.TgMessageId == y.TgMessageId) && (x.ChannelId == y.ChannelId))
            {
                return true;
            }

            return false;
        }

        public int GetHashCode(Post obj)
        {
            return obj.TgMessageId.GetHashCode();
        }
    }
}