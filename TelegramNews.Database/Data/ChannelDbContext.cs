namespace TelegramNews.Database.Data
{
    using Microsoft.EntityFrameworkCore;
    using Entities;

    public class ChannelDbContext : DbContext
    {
        public ChannelDbContext(DbContextOptions<ChannelDbContext> options)
            : base(options)
        { }

        public DbSet<Channel> Channels { get; set; }
        public DbSet<Post> Posts { get; set; }
    }
}