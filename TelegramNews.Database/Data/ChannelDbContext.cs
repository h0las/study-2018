namespace TelegramNews.Database.Data
{
    using Microsoft.EntityFrameworkCore;
    using Entities;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

    public class ChannelDbContext : IdentityDbContext<User>
    {
        public DbSet<Post> Posts { get; set; }
        public DbSet<Channel> Channels { get; set; }

        public ChannelDbContext(DbContextOptions<ChannelDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}