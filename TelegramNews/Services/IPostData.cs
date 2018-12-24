namespace TelegramNews.Services
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using TelegramNews.Database.Entities;

    public interface IPostData
    {
        IEnumerable<Post> GetAll();

        Post Get(int id);

        void Add(Post newPost);

        void Add(IEnumerable<Post> posts);

        int Commit();
    }
}