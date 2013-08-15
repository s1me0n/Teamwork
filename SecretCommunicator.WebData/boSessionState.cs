using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;
using System.Configuration;
using Spring.Social.OAuth1;
using Spring.Social.Dropbox.Connect;
using Spring.Social.Dropbox.Api;
using SecretCommunicator.WebData.Library;

namespace SecretCommunicator.WebData
{
    public class boSessionState
    {
        public IDropbox Client { get; set; }

        public User CurrentUser { get; set; }

        public MongoDatabase db
        {
            get
            {
                var connectionstring = ConfigurationManager.AppSettings["MONGOLAB_URI"];
                string dbName = MongoUrl.Create(connectionstring).DatabaseName;
                MongoServer dbServer = MongoServer.Create(connectionstring);
                MongoDatabase dbConnection = dbServer.GetDatabase(dbName, SafeMode.True);
                return dbConnection;
            }
        }
    }
}