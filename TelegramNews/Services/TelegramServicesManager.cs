namespace TelegramNews.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using TelegramNews.Database.Entities;
    using Microsoft.Extensions.Configuration;
    using TeleSharp.TL;
    using TeleSharp.TL.Messages;
    using TLSharp.Core;

    public class TelegramServicesManager : ITelegramServicesManager
    {
        private IConfiguration _configuration;
        private int _apiId;
        private string _apiHash;
        private TelegramClient _client;
        private string _hashCode;
        private string _lastCodeRequestNumber;

        public TelegramServicesManager(IConfiguration configuration)
        {
            _configuration = configuration;

            _apiHash = _configuration.GetSection("TelegramDevAccountConfig").GetSection("ApiHash").Value;
            _apiId = Convert.ToInt32(_configuration.GetSection("TelegramDevAccountConfig").GetSection("ApiId").Value);

            _client = GetTelegramClient();
        }

        public void ConnectClientToTelegramAsync()
        {
            var t = _client.ConnectAsync();
            t.Wait();
        }

        public async Task<IEnumerable<Post>> GetPosts(int limit, string channelName)
        {
            var resultMessages = new List<Post>();

            var offset = 0;
            var maxId = -1;

            var dialogs = (TLDialogs)await _client.GetUserDialogsAsync();
            var chat = dialogs.Chats.ToList()
                .OfType<TLChannel>()
                .FirstOrDefault(c => c.Title == channelName);

            if (chat.AccessHash != null)
            {
                var tlAbsMessages =
                    await _client.GetHistoryAsync(
                        new TLInputPeerChannel { ChannelId = chat.Id, AccessHash = chat.AccessHash.Value }, offset, maxId, limit);

                var tlChannelMessages = (TLChannelMessages)tlAbsMessages;

                for (var index = 0; index < tlChannelMessages.Messages.Count - 1; index++)
                {
                    var tlAbsMessage = tlChannelMessages.Messages.ToList()[index];
                    var message = (TLMessage)tlAbsMessage;
                    
                    if (message.Media == null)
                    {
                        resultMessages.Add(new Post()
                        {
                            TgMessageId = message.Id,
                            ChannelId = chat.Id,
                            Content = message.Message,
                            Type = EnChannelMessage.Message,
                            Views = message.Views,
                            ChannelName = channelName
                        });
                    }
                }
            }

            return resultMessages;
        }

        public bool IsUserAuthorized()
        {
            return _client.IsUserAuthorized();
        }

        public Task<TLUser> MakeAuthAsync(string code)
        {
            return Task.Run(() => _client.MakeAuthAsync(_lastCodeRequestNumber, _hashCode, code));
        }

        public void SendCodeRequestAsync(string phoneNumber)
        {
            _lastCodeRequestNumber = phoneNumber;
            ConnectClientToTelegramAsync();
            _hashCode = Task.Run(() => _client.SendCodeRequestAsync(phoneNumber)).Result;
        }

        private TelegramClient GetTelegramClient()
        {
            var store = new FileSessionStore();
            var session = "session_1230002233";

            return new TelegramClient(_apiId, _apiHash, store, session);
        }
    }
}