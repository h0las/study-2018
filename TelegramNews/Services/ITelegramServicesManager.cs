namespace TelegramNews.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using TelegramNews.Database.Entities;
    using TeleSharp.TL;
    using TLSharp.Core;

    public interface ITelegramServicesManager
    {
        Task<IEnumerable<Post>> GetPosts(int limit, int maxId, string channelName);

        void ConnectClientToTelegramAsync();

        void SendCodeRequestAsync(string phoneNumber);

        bool IsUserAuthorized();

        Task<TLUser> MakeAuthAsync(string code);
    }
}