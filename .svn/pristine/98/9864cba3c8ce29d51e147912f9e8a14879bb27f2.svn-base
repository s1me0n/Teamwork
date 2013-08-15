using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using MongoDB.Driver;
using SecretCommunicator.Models;
using SecretCommunicator.Models.Library;
using Spring.Social.Dropbox.Api;

namespace SecretCommunicator.Data
{
    public class SessionState
    {
        public User CurrentUser { get; set; }

        public IDropbox DropboxClient { get; set; }

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

        public PubnubAPI Pubnub = new PubnubAPI(
                "pub-c-b3df6166-8457-43e9-8962-77f0537f9739",				// PUBLISH_KEY
                "sub-c-8fe6f99c-e1cd-11e2-95e6-02ee2ddab7fe",				// SUBSCRIBE_KEY
                "sec-c-NGRmMGFmODgtOWEyOS00Y2Y2LTkyMWEtODI3Mzc4YzQ3YTY4", // SECRET_KEY
                true													// SSL_ON?
                );
    }
}
