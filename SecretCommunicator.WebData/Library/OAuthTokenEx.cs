using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;

namespace SecretCommunicator.WebData.Library
{
    public class OAuthTokenEx
    {
        public ObjectId Id { get; set; }

        public dynamic PrivateData { get; set; }
    }
}