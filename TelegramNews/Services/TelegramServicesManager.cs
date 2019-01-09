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
    using TeleSharp.TL.Upload;
    using TLSharp.Core;
    using System.IO;

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

        public async Task<IEnumerable<Post>> GetPosts(int limit, int maxId, string channelName)
        {
            var resultMessages = new List<Post>();

            var offset = 0;

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
                    else
                    {
                        switch (message.Media.GetType().ToString())
                        {
                            case "TeleSharp.TL.TLMessageMediaPhoto":
                                var tLMessageMediaPhoto = (TLMessageMediaPhoto)message.Media;
                                var photo = (TLPhoto)tLMessageMediaPhoto.Photo;

                                var resPhoto = await GetPhotoAttachmentBytes(photo);

                                resultMessages.Add(new Post()
                                {
                                    TgMessageId = message.Id,
                                    ChannelId = chat.Id,
                                    Content = tLMessageMediaPhoto.Caption,
                                    Type = EnChannelMessage.MediaPhoto,
                                    Views = message.Views ?? 0,
                                    ChannelName = channelName,
                                    File = resPhoto,
                                    FileType = "image/jpeg"
                                });
                                break;
                            case "TeleSharp.TL.TLMessageMediaWebPage":
                                var tLMessageMediaWebPage = (TLMessageMediaWebPage)message.Media;
                                var url = string.Empty;
                                var title = string.Empty;

                                if (tLMessageMediaWebPage.Webpage.GetType().ToString() != "TeleSharp.TL.TLWebPageEmpty")
                                {
                                    var webPage = (TLWebPage)tLMessageMediaWebPage.Webpage;
                                    url = webPage.Url;
                                    title = webPage.Title;
                                }

                                resultMessages.Add(new Post
                                {
                                    TgMessageId = message.Id,
                                    ChannelId = chat.Id,
                                    Content = message.Message,
                                    Type = EnChannelMessage.WebPage,
                                    Views = message.Views ?? 0,
                                    ChannelName = channelName,
                                    Url = url,
                                    Title = title
                                });
                                break;
                            default:
                                break;
                            case "TeleSharp.TL.TLMessageMediaDocument":
                                var tLMessageMediaDocument = (TLMessageMediaDocument)message.Media;
                                var document = (TLDocument)tLMessageMediaDocument.Document;

                                var resDocument = await GetDocumentAttachmentBytes(document);
                                resultMessages.Add(new Post()
                                {
                                    TgMessageId = message.Id,
                                    ChannelId = chat.Id,
                                    Content = tLMessageMediaDocument.Caption,
                                    Type = EnChannelMessage.MediaDocument,
                                    Views = message.Views ?? 0,
                                    ChannelName = channelName,
                                    File = resDocument,
                                    FileType = document.MimeType
                                });
                                break;
                        }
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
            var session = "session_1234567890";

            return new TelegramClient(_apiId, _apiHash, store, session);
        }

        private async Task<byte[]> GetPhotoAttachmentBytes(TLPhoto photo)
        {
            var photoSize = photo.Sizes.ToList().OfType<TLPhotoSize>().Last();
            var tlFileLocation = (TLFileLocation)photoSize.Location;

            var filepart = 512 * 1024;
            var offset = 0;
            var resFile = new TLFile();

            using (var ms = new MemoryStream())
            {
                while(offset < photoSize.Size)
                {
                    resFile = await _client.GetFile(
                        new TLInputFileLocation
                        {
                            LocalId = tlFileLocation.LocalId,
                            Secret = tlFileLocation.Secret,
                            VolumeId = tlFileLocation.VolumeId
                        }, filepart, offset
                    );

                    ms.Write(resFile.Bytes, 0, resFile.Bytes.Length);
                    offset += filepart;
                }

                resFile.Bytes = ms.ToArray();
            }   

            return resFile.Bytes;
        }

        private async Task<byte[]> GetDocumentAttachmentBytes(TLDocument document)
        {
            var filepart = 512 * 1024;
            var offset = 0;
            var resFile = new TLFile();

            using (var ms = new MemoryStream())
            {
                while (offset < document.Size)
                {
                    resFile = await _client.GetFile(
                    new TLInputDocumentFileLocation
                    {
                        AccessHash = document.AccessHash,
                        Id = document.Id,
                        Version = document.Version
                    }, filepart, offset
                );

                    ms.Write(resFile.Bytes, 0, resFile.Bytes.Length);
                    offset += filepart;
                }

                resFile.Bytes = ms.ToArray();
            }

            return resFile.Bytes;
        }
    }
}