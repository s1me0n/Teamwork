using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using Newtonsoft.Json;
using SecretCommunicator.Models;
using SecretCommunicator.Models.Library;
using Spring.IO;
using Spring.Social.Dropbox.Api;
using Spring.Social.Dropbox.Connect;
using Spring.Social.OAuth1;

namespace SecretCommunicator.Data
{
    public static class Extensions
    {
        #region Users

        public static IQueryable<User> GetUsers(this SessionState _sessionState)
        {
            return _sessionState.db.GetCollection<User>("User").AsQueryable();
        }

        public static SafeModeResult SaveUser(this SessionState _sessionState, Channel value)
        {
            var result = _sessionState.db.GetCollection<User>("User").Save(value, SafeMode.True);
            if (result.Ok)
                AppCache.ChannelList.AddSafeName(value);
            return result;
        }

        public static void DeleteUsers(this SessionState _sessionState)
        {
            _sessionState.db.DropCollection("User");
        }

        #endregion

        #region Channels

        public static IQueryable<Channel> GetChannels(this SessionState _sessionState)
        {
            return _sessionState.db.GetCollection<Channel>("Channel").AsQueryable();
        }

        public static SafeModeResult SaveChannel(this SessionState _sessionState, Channel value)
        {
            var result = _sessionState.db.GetCollection<Channel>("Channel").Save(value, SafeMode.True);
            if (result.Ok)
                AppCache.ChannelList.AddSafeName(value);
            return result;
        }

        public static void DeleteChannels(this SessionState _sessionState)
        {
            _sessionState.db.DropCollection("Channel");
        }

        #endregion

        #region Messages

        public static IQueryable<Message> GetMessages(this SessionState _sessionState)
        {
            return _sessionState.db.GetCollection<Message>("Message").AsQueryable();
        }

        public static SafeModeResult SaveMessage(this SessionState _sessionState, Message value)
        {
            var result = _sessionState.db.GetCollection<Message>("Message").Save(value, SafeMode.True);
            if (result.Ok)
                AppCache.MessageList.AddSafeName(Functions.CreatePublicMessage(value));
            return result;
        }

        public static SafeModeResult DeleteMessage(this SessionState _sessionState, Message value)
        {
            var result = _sessionState.db.GetCollection<Message>("Message").Remove(Query.EQ("_id", value.Id));
            if (result.Ok)
                AppCache.MessageList.Remove(value);
            return result;
        }

        public static void DeleteMessage(this SessionState _sessionState)
        {
            _sessionState.db.DropCollection("Message");
        }

        #endregion

        #region OAuth

        public static IQueryable<OAuthTokenEx> GetOAuthToken(this SessionState _sessionState)
        {
            return _sessionState.db.GetCollection<OAuthTokenEx>("OAuthTokenEx").AsQueryable();
        }

        public static SafeModeResult SaveOAuthToken(this SessionState _sessionState, OAuthTokenEx value)
        {
            return _sessionState.db.GetCollection<OAuthTokenEx>("OAuthTokenEx").Save(value, SafeMode.True);
        }

        public static void DeleteOAuthToken(this SessionState _sessionState)
        {
            _sessionState.db.DropCollection("OAuthTokenEx");
        }

        #endregion

        #region Other

        public static string DropboxShareFile(this SessionState _sessionState, string path, string filename)
        {
            var appAuth = new Tuple<string, string>("5hq4n8kjyopqzje", "22h4xl0x1g569af");
            var userAuth = new Tuple<string, string>("higmkxi48pfhv8jt", "0wouwudro53wmkz");

            var dropboxServiceProvider = new DropboxServiceProvider(appAuth.Item1, appAuth.Item2, AccessLevel.AppFolder);
            var dropbox = dropboxServiceProvider.GetApi(userAuth.Item1, userAuth.Item2);
            var uploadFileEntry = dropbox.UploadFileAsync(new FileResource(path), filename).Result;
            var sharedUrl = dropbox.GetMediaLinkAsync(uploadFileEntry.Path).Result;
            
            return (sharedUrl.Url + "?dl=1"); // we can download the file directly
        }

        public static void DropboxAuthEnd(this SessionState _sessionState)
        {
            AuthorizedRequestToken authorizedRequestToken = new AuthorizedRequestToken(AppCache.requestToken, null);
            AppCache.token = AppCache.dropboxProvider.OAuthOperations.ExchangeForAccessTokenAsync(authorizedRequestToken, null).Result;
            OAuthTokenEx o = _sessionState.GetOAuthToken().FirstOrDefault();
            if (o == null)
            {
                o = new OAuthTokenEx();
                _sessionState.SaveNewToken(o);
            }
            else
            {
                dynamic pd = (new JsonSerializer()).Deserialize(new JsonTextReader(new StringReader(AppCache.AESProvider.DecryptString(o.PrivateData))));
                if (pd.Secret.Value != AppCache.token.Secret || pd.Value.Value != AppCache.token.Value)
                    _sessionState.SaveNewToken(o);
            }
        }

        public static void SaveNewToken(this SessionState _sessionState, OAuthTokenEx o)
        {
            o.PrivateData = AppCache.AESProvider.EncryptToString(JsonConvert.SerializeObject(new OAuthTokenExResource() { Secret = AppCache.token.Secret, Value = AppCache.token.Value }));
            _sessionState.SaveOAuthToken(o);
        }

        public static void AuthClient(this SessionState _sessionState)
        {
            if (_sessionState.DropboxClient == null)
            {
                OAuthTokenEx o = _sessionState.GetOAuthToken().FirstOrDefault();
                if (o != null)
                {
                    dynamic pd = (new JsonSerializer()).Deserialize(new JsonTextReader(new StringReader(AppCache.AESProvider.DecryptString(o.PrivateData))));
                    _sessionState.DropboxClient = AppCache.dropboxProvider.GetApi(pd.Value.Value, pd.Secret.Value);
                }
            }
        }

        public static void AddSafeName(this List<Channel> l, Channel value)
        {
            if (l.Find(c => c.Name == value.Name) == null)
                l.Add(value);
        }

        public static void AddSafeName(this List<Message> l, Message value)
        {
            if (l.Find(c => c.Id == value.Id) == null)
                l.Add(Functions.CreatePublicMessage(value));
        }

        public static void GetChanMessages(this SessionState _sessionState, Channel chan, int numberOfMessages)
        {
            if (AppCache.MessageList.Where(m => m.ChannelId == chan.Id).OrderBy(m => m.CreatedDateTime).Take(numberOfMessages).ToList().Count == 0)
            {
                _sessionState.GetMessages().Where(m => m.ChannelId == chan.Id).OrderBy(m => m.CreatedDateTime).Take(numberOfMessages).ToList().ForEach(m =>
                {
                    m.Status = "add";
                    //if (m.Type == MessageTypes.File)
                    //{
                    //    _sessionState.AuthClient();
                    //    var url = _sessionState.DropboxClient.GetMediaLinkAsync(m.Content).Result.Url;
                    //    m.Content = url;
                    //}
                    AppCache.MessageList.AddSafeName(m);
                });
            }
        }

        public static Channel GetChannel(this SessionState _sessionState, string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;
            Channel result = AppCache.ChannelList.Where(c => c.Name == name).FirstOrDefault();
            if (result == null)
            {
                result = _sessionState.GetChannels().Where(c => c.Name == name).FirstOrDefault();
                if (result == null)
                    result = new Channel();
                AppCache.ChannelList.AddSafeName(result);
            }
            return result;
        }

        public static Channel GetChannel(this SessionState _sessionState, string name, string password, PubnubAPI pubnub)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(password))
                return null;
            Channel result = AppCache.ChannelList.Where(c => c.Name == name && c.Password == password).FirstOrDefault();
            if (result == null)
            {
                //try to get channel from mongodb
                result = _sessionState.GetChannels().Where(c => c.Name == name).FirstOrDefault();// && c.PublicData.Password == password).FirstOrDefault();
                if (result == null)
                {
                    //save new channel
                    result = new Channel();
                    result.Name = name;
                    result.Password = password;
                    result.CreatedDateTime = DateTime.Now;
                    _sessionState.SaveChannel(result);
                    //add channel in cache
                    AppCache.ChannelList.AddSafeName(result);
                    if (pubnub != null)
                    {
                        List<object> publishAllChannel = pubnub.Publish("SCAllChannel", result.Name);
                    }
                }
                else if(result.Password != password)
                    result = null;
                
            }
            return result;
        }

        #endregion
    }
}