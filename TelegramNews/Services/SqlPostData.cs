namespace TelegramNews.Services
{
    using System;
    using System.Collections.Generic;
    using TelegramNews.Database.Entities;
    using TelegramNews.Database.Data;

    public class SqlPostData : IPostData
    {
        private ChannelDbContext _db;

        public SqlPostData(ChannelDbContext db)
        {
            _db = db;
        }

        public void Add(Post newPost)
        {
            _db.Add(newPost);
            Commit();
        }

        public void Add(IEnumerable<Post> posts)
        {
            foreach(var post in posts)
            {
                Add(post);
            }
        }

        public int Commit()
        {
            return _db.SaveChanges();
        }

        public Post Get(int id)
        {
            return _db.Find<Post>(id);
        }

        public IEnumerable<Post> GetAll()
        {
            return _db.Posts;
        }
    }
}