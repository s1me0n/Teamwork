using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SecretCommunicator.WebData;

namespace SecretCommunicator.WebApp
{
    public partial class DeleteAllMessages : System.Web.UI.Page
    {
        boSessionState _sessionState = new boSessionState();

        protected void Page_Load(object sender, EventArgs e)
        {
            _sessionState.DeleteMessage();
            AppCache.MessageList.Clear();
            Response.Write("done");
        }
    }
}