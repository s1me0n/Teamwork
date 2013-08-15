using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.SessionState;
using SecretCommunicator.Data;
using SecretCommunicator.Data.Helpers;
using SecretCommunicator.Data.Interfaces;
using SecretCommunicator.Models;
using SecretCommunicator.Models.Library;

namespace SecretCommunicator.WebApi.Controllers
{
    public class ChannelController : BaseApiController<Channel>, IRequiresSessionState
    {
        SessionState _sessionState = new SessionState();
        RestoreSession _restoreSession = new RestoreSession() { GetUserChannels = new List<string>() };

        public ChannelController(IRepository repository)
            : base(repository)
        {
        }

        public override HttpResponseMessage Post(Channel value)
        {
            try
            {
                GetSession();
                var channel = new Channel();
                if (_sessionState.CurrentUser != null && _sessionState.CurrentUser.ChannelName.Contains(value.Name))
                {
                    channel = _sessionState.GetChannel(value.Name);
                    if (channel != null)
                    {
                        _sessionState.GetChanMessages(channel, 10);
                        channel.Messages = AppCache.MessageList.Where(m => m.ChannelId == channel.Id).Take(10).ToList();
                    }
                }
                else
                {
                    channel = _sessionState.GetChannel(value.Name, value.Password, _sessionState.Pubnub);
                    if (channel != null)
                    {
                        if (_sessionState.CurrentUser == null)
                            _sessionState.CurrentUser = new User()
                            {
                                Id = Guid.NewGuid(),
                                ChannelName = new List<string>()
                            };
                        if (!_sessionState.CurrentUser.ChannelName.Contains(value.Name))
                        {
                            _sessionState.CurrentUser.ChannelName.Add(value.Name);
                        }

                        _sessionState.GetChanMessages(channel, value.NumberOfMessages);

                        channel.Messages =
                            AppCache.MessageList.Where(m => m.ChannelId == channel.Id)
                                .OrderBy(m => m.CreatedDateTime)
                                .Take(value.NumberOfMessages)
                                .ToList();
                        if (_restoreSession.LastChannel == null)
                            _restoreSession.LastChannel = new Channel();
                        _restoreSession.LastChannel = channel;
                    }
                    else
                    {
                        var errResponse = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                            "If create a new channel: channel exist." + Environment.NewLine +
                            "If login to a channel: password not match.");
                        throw new HttpResponseException(errResponse);
                    }
                }
                
                var response =
                    Request.CreateResponse(HttpStatusCode.Created, channel);

                response.Headers.Location = new Uri(Url.Link("DefaultApi", new {id = value.Id}));
                return response;
            }
            catch (Exception ex)
            {
                var errResponse = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
                throw new HttpResponseException(errResponse);
            }
            finally
            {
                SaveSession();
            }
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
