using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web.Services;
using SecretCommunicator.WebData.Library;
using System.Web;
using SecretCommunicator.WebData;
using Newtonsoft.Json;

namespace SecretCommunicator.WebApp.Webservice
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ChannelService
    {
        PubnubAPI pubnub = new PubnubAPI(
               "pub-2e41c0cf-3a9d-4d2e-83d2-1d7ae0a12b78",				// PUBLISH_KEY
               "sub-c29f5187-b88e-11e1-9907-d99166562f9b",				// SUBSCRIBE_KEY
               "sec-OWVhZjIwYTMtNDZjYy00NzdkLTk0M2QtZWQzY2U2OTU2MWE1", // SECRET_KEY
               true													// SSL_ON?
               );

        boSessionState _sessionState = new boSessionState();
        RestoreSession _restoreSession = new RestoreSession() { GetUserChannels = new List<string>(), LastChannel = new List<dynamic>() };

        [WebMethod(EnableSession = true)]
        [WebGet(UriTemplate = "restoreSession")]
        public string RestoreSession()
        {
            ServiceData sd = new ServiceData() { ResultType = "restoreSession" };
            try
            {
                GetSession();
                if (_sessionState.CurrentUser != null && _sessionState.CurrentUser.ChannelName.Count > 0)
                    _restoreSession.GetUserChannels = _sessionState.CurrentUser.ChannelName;
                sd.Result = _restoreSession;
            }
            catch (Exception ex)
            {
                sd.IsError = true;
                sd.ErrorMessage = ex.Message;
            }
            finally
            {
                SaveSession();
            }
            return JsonConvert.SerializeObject(sd);
        }

        [WebMethod(EnableSession = true)]
        [WebGet(UriTemplate = "getAllChannels")]
        public string GetAllChannels()
        {
            ServiceData sd = new ServiceData() { ResultType = "getAllChannels" };
            try
            {
                GetSession();
                //_sessionState.DeleteMessage();
                //_sessionState.DeleteChannels();
                if (AppCache.ChannelList.Count == 0)
                    AppCache.ChannelList = _sessionState.GetChannels().ToList();
                if(sd.Result == null)
                    sd.Result = new List<string>();
                if(AppCache.ChannelList.Count > 0)
                    sd.Result.AddRange(AppCache.ChannelList.Select(c => c.Name));
            }
            catch (Exception ex)
            {
                sd.IsError = true;
                sd.ErrorMessage = ex.Message + Environment.NewLine + ex.StackTrace;
            }
            finally
            {
                SaveSession();
            }
            return JsonConvert.SerializeObject(sd);
        }

        [WebMethod(EnableSession = true)]
        [WebInvoke(Method = "POST", UriTemplate = "getUserChannel", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        public string GetUserChannel(string name)
        {
            ServiceData sd = new ServiceData() { ResultType = "getUserChannel" };
            try
            {
                GetSession();
                if (_sessionState.CurrentUser != null && _sessionState.CurrentUser.ChannelName.Contains(name))
                {
                    Channel chan = _sessionState.GetChannel(name);
                    if (chan != null)
                    {
                        _sessionState.GetChanMessages(chan, 10);
                        sd.Result = new List<dynamic>();
                        sd.Result.Add(chan);
                        sd.Result.Add(AppCache.MessageList.Where(m => m.ChannelId == chan.Id).Take(10).ToList());
                    }
                }
                else
                {
                    sd.IsError = true;
                    if (_sessionState.CurrentUser == null)
                        sd.ErrorMessage = "You are not login";
                    else if(!_sessionState.CurrentUser.ChannelName.Contains(name))
                        sd.ErrorMessage = "Login to the channel please";
                }
                if (_restoreSession.LastChannel == null)
                    _restoreSession.LastChannel = new List<dynamic>();
                _restoreSession.LastChannel = sd.Result;
            }
            catch (Exception ex)
            {
                sd.IsError = true;
                sd.ErrorMessage = ex.Message;
            }
            finally
            {
                SaveSession();
            }
            return JsonConvert.SerializeObject(sd);
        }

        [WebMethod(EnableSession = true)]
        [WebInvoke(Method = "POST", UriTemplate = "joinCreateChannel", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        public string JoinCreateChannel(string name, string password, int numberOfMessages)
        {
            ServiceData sd = new ServiceData() { ResultType = "joinCreateChannel" };
            try
            {
                GetSession();
                Channel chan = _sessionState.GetChannel(name, password, pubnub);
                if (chan != null)
                {
                    if (_sessionState.CurrentUser == null)
                        _sessionState.CurrentUser = new User() { Id = Guid.NewGuid(), ChannelName = new List<string>() };
                    if (!_sessionState.CurrentUser.ChannelName.Contains(name))
                    {
                        _sessionState.CurrentUser.ChannelName.Add(name);
                    }

                    _sessionState.GetChanMessages(chan, numberOfMessages);
                    sd.Result = new List<dynamic>();
                    sd.Result.Add(chan);
                    sd.Result.Add(AppCache.MessageList.Where(m => m.ChannelId == chan.Id).OrderBy(m => m.PrivateDateTime).Take(numberOfMessages).ToList());
                    if (_restoreSession.LastChannel == null)
                        _restoreSession.LastChannel = new List<dynamic>();
                    _restoreSession.LastChannel = sd.Result;
                }
                else
                {
                    sd.IsError = true;
                    sd.ErrorMessage = "If create a new channel: channel exist." + Environment.NewLine + "If login to a channel: password not match.";
                }
            }
            catch (Exception ex)
            {
                sd.IsError = true;
                sd.ErrorMessage = ex.Message;
            }
            finally
            {
                SaveSession();
            }
            return JsonConvert.SerializeObject(sd);
        }

        [WebMethod(EnableSession = true)]
        [WebInvoke(Method = "POST", UriTemplate = "deletePost", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        public string DeletePost(string name, string password, string messageId)
        {
            ServiceData sd = new ServiceData() { ResultType = "deletePost" };
            try
            {
                GetSession();
                if (_sessionState.CurrentUser != null && _sessionState.CurrentUser.ChannelName.Contains(name))
                {
                    Channel chan = _sessionState.GetChannel(name);
                    if (chan != null)
                    {
                        Message msg = AppCache.MessageList.Where(m => m.Id == messageId).FirstOrDefault();
                        if (msg == null)
                            msg = _sessionState.GetMessages().Where(m => m.Id == messageId).FirstOrDefault();
                        if (msg != null)
                        {
                            _sessionState.DeleteMessage(msg);
                            PublishToPubNub(name, sd, true);
                        }
                    }
                }
                else
                {
                    Channel chan = _sessionState.GetChannel(name, password, pubnub);
                    if (chan != null)
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                sd.IsError = true;
                sd.ErrorMessage = ex.Message;
            }
            finally
            {
                SaveSession();
            }

            return JsonConvert.SerializeObject(sd);
        }

        [WebMethod(EnableSession = true)]
        [WebInvoke(Method = "POST", UriTemplate = "postText", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        public string PostText(string name, string password, string message)
        {
            ServiceData sd = new ServiceData() { ResultType = "postText" };
            try
            {
                GetSession();
                sd.Result = PostContent(name, password, message, MessageTypes.Text);
            }
            catch (Exception ex)
            {
                sd.IsError = true;
                sd.ErrorMessage = ex.Message;
            }
            finally
            {
                SaveSession();
            }
            PublishToPubNub(name, sd, false);
            return JsonConvert.SerializeObject(sd);
        }

        [WebMethod(EnableSession = true)]
        [WebInvoke(Method = "POST", UriTemplate = "postLink", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        public string PostLink(string name, string password, string linkUrl, string linkTitle)
        {
            ServiceData sd = new ServiceData() { ResultType = "postLink" };
            try
            {
                string message = linkUrl + "|" + linkTitle;
                sd.Result = PostContent(name, password, message, MessageTypes.Link);
            }
            catch (Exception ex)
            {
                sd.IsError = true;
                sd.ErrorMessage = ex.Message;
            }
            finally
            {
                SaveSession();
            }
            PublishToPubNub(name, sd, false);
            return JsonConvert.SerializeObject(sd);
        }

        private Message PostContent(string name, string password, string message, MessageTypes messageType)
        {
            GetSession();
            if (_sessionState.CurrentUser == null)
            {
                Channel chan = _sessionState.GetChannel(name, password, pubnub);
                if (chan != null)
                {
                    _sessionState.CurrentUser = new User() { Id = Guid.NewGuid(), ChannelName = new List<string>() };
                    _sessionState.CurrentUser.ChannelName.Add(name);
                    return AddMessage(chan, message, messageType);
                }
                //else
                //    result = "Error:Channel name or password not match";
            }
            else
            {
                if (_sessionState.CurrentUser.ChannelName.Find(c => c.Equals(name)).Count() > 0)
                {
                    Channel chan = _sessionState.GetChannel(name);
                    if (chan != null)
                        return AddMessage(chan, message, messageType);
                    //else
                    //    result = "Error:Channel name Not Found";
                }
                else
                {
                    Channel chan = _sessionState.GetChannel(name, password, pubnub);
                    if (chan != null)
                    {
                        _sessionState.CurrentUser.ChannelName.Add(name);
                        return AddMessage(chan, message, messageType);
                    }
                    //else
                    //    result = "Error:Channel name or password not match";
                }
            }
            SaveSession();
            return new Message();
        }

        private Message AddMessage(Channel chan, string message, MessageTypes type)
        {
            Message msg = new Message();
            msg.ChannelId = chan.Id;
            msg.PrivateDateTime = DateTime.Now;
            msg.CreatedDateTime = msg.PrivateDateTime.ToString();
            msg.PublicData = new MessageResource() { Type = type, Value = message };
            msg.PrivateData = AppCache.AESProvider.EncryptToString(JsonConvert.SerializeObject(msg.PublicData));
            _sessionState.SaveMessage(msg);
            AppCache.MessageList.AddSafeName(msg);
            SaveSession();
            return msg;
        }

        private void PublishToPubNub(string name, ServiceData sd, bool isRemove)
        {
            Message msg = sd.Result as Message;
            if (msg != null)
            {
                List<dynamic> pubnubMessage = new List<dynamic>();
                Dictionary<string, string> actionDict = new Dictionary<string, string>();
                if(isRemove)
                    actionDict.Add("Action", "del");
                else
                    actionDict.Add("Action", "add");
                actionDict.Add("Channel", name);
                pubnubMessage.Add(actionDict);
                msg.PrivateData = null;
                pubnubMessage.Add(msg);
                List<object> publishResult = pubnub.Publish("NewMsgIn" + name, pubnubMessage);
            }
        }

        private void GetSession()
        {
            if (HttpContext.Current.Session["sessionState"] != null)
                _sessionState = (boSessionState)HttpContext.Current.Session["sessionState"];

            if (HttpContext.Current.Session["restoreSession"] != null)
                _restoreSession = (RestoreSession)HttpContext.Current.Session["restoreSession"];
        }

        protected void SaveSession()
        {
            HttpContext.Current.Session["sessionState"] = _sessionState;
            HttpContext.Current.Session["restoreSession"] = _restoreSession;
        }
    }
}
