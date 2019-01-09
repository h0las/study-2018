namespace TelegramNews.Services
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using TelegramNews.Database.Entities;

    public interface IPostData
    {
        IEnumerable<Post> GetAll();

        IEnumerable<Channel> GetAllChannels();

        Post Get(int id);

        Channel GetChannel (int id);

        void Add(Post newPost);

        void Add(Channel newPost);

        void Add(IEnumerable<Post> posts);

        int Commit();
    }
}