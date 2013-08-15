using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SecretCommunicator.WebData;
using SecretCommunicator.WebData.Library;
using Spring.Social.OAuth1;
using System.Configuration;

namespace SecretCommunicator.WebApp
{
    public partial class Authorize : System.Web.UI.Page
    {
        boSessionState _sessionState = new boSessionState();

        protected void Page_Init(object sender, EventArgs e)
        {
            if (Session["sessionState"] != null)
                _sessionState = (boSessionState)Session["sessionState"];
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //_sessionState.DeleteOAuthToken();
            if (!Page.IsPostBack)
            {
                if (Request.Params["dropboxcallback"] == "1")
                    _sessionState.DropboxAuthEnd();
                else
                {
                    if (_sessionState.Client == null)
                    {
                        OAuthTokenEx o = _sessionState.GetOAuthToken().FirstOrDefault();
                        if (o == null)
                            DropboxAuthStart();
                        else
                            _sessionState.AuthClient();
                    }
                }
                SaveSession();
            }
        }

        protected void PreRender(EventHandler e)
        {
            SaveSession();
        }
        
        protected void SaveSession()
        {
            Session["sessionState"] = _sessionState;
        }

        private void DropboxAuthStart()
        {
            OAuth1Parameters parameters = new OAuth1Parameters();
            parameters.CallbackUrl = ConfigurationManager.AppSettings["DropboxCallback"] + "?dropboxcallback=1";
            SaveSession();
            Response.Redirect(AppCache.dropboxProvider.OAuthOperations.BuildAuthenticateUrl(AppCache.requestToken.Value, parameters));
        }
    }
}