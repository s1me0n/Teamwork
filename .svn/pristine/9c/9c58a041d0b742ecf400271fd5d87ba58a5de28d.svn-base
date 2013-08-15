using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using SecretCommunicator.WebData.Library;
using Spring.Social.OAuth1;
using MongoDB.Bson;
using Newtonsoft.Json;
using System.IO;
using MongoDB.Driver.Builders;

namespace SecretCommunicator.WebData
{
    public static class Extensions
    {
        #region Users

        public static IQueryable<User> GetUsers(this boSessionState _sessionState)
        {
            return _sessionState.db.GetCollection<User>("User").AsQueryable();
        }

        public static SafeModeResult SaveUser(this boSessionState _sessionState, Channel value)
        {
            var result = _sessionState.db.GetCollection<User>("User").Save(value, SafeMode.True);
            if (result.Ok)
                AppCache.ChannelList.AddSafeName(value);
            return result;
        }

        public static void DeleteUsers(this boSessionState _sessionState)
        {
            _sessionState.db.DropCollection("User");
        }

        #endregion

        #region Channels

        public static IQueryable<Channel> GetChannels(this boSessionState _sessionState)
        {
            return _sessionState.db.GetCollection<Channel>("Channel").AsQueryable();
        }

        public static SafeModeResult SaveChannel(this boSessionState _sessionState, Channel value)
        {
            var result = _sessionState.db.GetCollection<Channel>("Channel").Save(value, SafeMode.True);
            if (result.Ok)
                AppCache.ChannelList.AddSafeName(value);
            return result;
        }

        public static void DeleteChannels(this boSessionState _sessionState)
        {
            _sessionState.db.DropCollection("Channel");
        }

        #endregion

        #region Messages

        public static IQueryable<Message> GetMessages(this boSessionState _sessionState)
        {
            return _sessionState.db.GetCollection<Message>("Message").AsQueryable();
        }

        public static SafeModeResult SaveMessage(this boSessionState _sessionState, Message value)
        {
            var result = _sessionState.db.GetCollection<Message>("Message").Save(value, SafeMode.True);
            if (result.Ok)
                AppCache.MessageList.AddSafeName(Functions.CreatePublicMessage(value));
            return result;
        }

        public static SafeModeResult DeleteMessage(this boSessionState _sessionState, Message value)
        {
            var result = _sessionState.db.GetCollection<Message>("Message").Remove(Query.EQ("_id", value.Id));
            if (result.Ok)
                AppCache.MessageList.Remove(value);
            return result;
        }

        public static void DeleteMessage(this boSessionState _sessionState)
        {
            _sessionState.db.DropCollection("Message");
        }

        #endregion

        #region OAuth

        public static IQueryable<OAuthTokenEx> GetOAuthToken(this boSessionState _sessionState)
        {
            return _sessionState.db.GetCollection<OAuthTokenEx>("OAuthTokenEx").AsQueryable();
        }

        public static SafeModeResult SaveOAuthToken(this boSessionState _sessionState, OAuthTokenEx value)
        {
            return _sessionState.db.GetCollection<OAuthTokenEx>("OAuthTokenEx").Save(value, SafeMode.True);
        }

        public static void DeleteOAuthToken(this boSessionState _sessionState)
        {
            _sessionState.db.DropCollection("OAuthTokenEx");
        }

        #endregion

        #region Other

        public static void DropboxAuthEnd(this boSessionState _sessionState)
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

        public static void SaveNewToken(this boSessionState _sessionState, OAuthTokenEx o)
        {
            o.PrivateData = AppCache.AESProvider.EncryptToString(JsonConvert.SerializeObject(new OAuthTokenExResource() { Secret = AppCache.token.Secret, Value = AppCache.token.Value }));
            _sessionState.SaveOAuthToken(o);
        }

        public static void AuthClient(this boSessionState _sessionState)
        {
            if (_sessionState.Client == null)
            {
                OAuthTokenEx o = _sessionState.GetOAuthToken().FirstOrDefault();
                if (o != null)
                {
                    dynamic pd = (new JsonSerializer()).Deserialize(new JsonTextReader(new StringReader(AppCache.AESProvider.DecryptString(o.PrivateData))));
                    _sessionState.Client = AppCache.dropboxProvider.GetApi(pd.Value.Value, pd.Secret.Value);
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

        public static void GetChanMessages(this boSessionState _sessionState, Channel chan, int numberOfMessages)
        {
            if (AppCache.MessageList.Where(m => m.ChannelId == chan.Id).OrderBy(m => m.PrivateDateTime).Take(numberOfMessages).ToList().Count == 0)
            {
                _sessionState.GetMessages().Where(m => m.ChannelId == chan.Id).OrderBy(m => m.PrivateDateTime).Take(numberOfMessages).ToList().ForEach(m =>
                {
                    if (m.PublicData.Type == MessageTypes.File)
                    {
                        _sessionState.AuthClient();
                        m.PrivateData = null;
                        var url = _sessionState.Client.GetMediaLinkAsync(m.PublicData.Value).Result.Url;
                        m.PublicData.Value = url;
                    }
                    AppCache.MessageList.AddSafeName(m);
                });
            }
        }

        public static Channel GetChannel(this boSessionState _sessionState, string name)
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

        public static Channel GetChannel(this boSessionState _sessionState, string name, string password, PubnubAPI pubnub)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(password))
                return null;
            Channel result = AppCache.ChannelList.Where(c => c.Name == name && c.PublicData.Password == password).FirstOrDefault();
            if (result == null)
            {
                //try to get channel from mongodb
                result = _sessionState.GetChannels().Where(c => c.Name == name).FirstOrDefault();// && c.PublicData.Password == password).FirstOrDefault();
                if (result == null)
                {
                    //save new channel
                    result = new Channel();
                    result.Name = name;
                    result.PublicData = new ChannelResource() { Password = password, CreatedDateTime = DateTime.Now };
                    result.PrivateData = AppCache.AESProvider.EncryptToString(JsonConvert.SerializeObject(result.PublicData));
                    _sessionState.SaveChannel(result);
                    //add channel in cache
                    AppCache.ChannelList.AddSafeName(result);
                    if (pubnub != null)
                    {
                        List<object> publishAllChannel = pubnub.Publish("SCAllChannel", result.Name);
                    }
                }
                else if(result.PublicData.Password != password)
                    result = null;
                
            }
            return result;
        }

        #endregion
    }
}