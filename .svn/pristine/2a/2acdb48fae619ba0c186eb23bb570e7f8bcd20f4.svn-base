using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using SecretCommunicator.Data;
using SecretCommunicator.Data.Helpers;
using SecretCommunicator.Data.Interfaces;
using SecretCommunicator.Models.Library;

namespace SecretCommunicator.WebApi.Controllers
{
    public class MessageController : BaseApiController<Message>
    {
        SessionState _sessionState = new SessionState();
        RestoreSession _restoreSession = new RestoreSession() { GetUserChannels = new List<string>() };

        public MessageController(IRepository repository)
            : base(repository)
        {
        }

        public override HttpResponseMessage Post(Message value)
        {
            var result = new HttpResponseMessage();
            if (!string.IsNullOrEmpty(value.Title) && !string.IsNullOrEmpty(value.Content))
            {
                value.CreatedDateTime = DateTime.Now.ToString();
                result = base.Post(value);
                PublishToPubNub(value, false);
            }
            return result;
        }

        public override void Delete(string id)
        {
            AppCache.MessageList.Remove(Get(id));
            DataStore.Delete<Message>(id);
        }

        private void PublishToPubNub(Message value, bool isRemove)
        {
            if (isRemove)
            {
                value.Status = "del";
            }
            else
            {
                value.Status = "add";
            }
            List<object> publishResult = _sessionState.Pubnub.Publish("NewMsgIn" + value.ChannelId, value);
        }

        private void GetSession()
        {
            if (HttpContext.Current.Session["sessionState"] != null)
                _sessionState = (SessionState)HttpContext.Current.Session["sessionState"];

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
