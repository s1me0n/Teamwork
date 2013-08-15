using System.Collections.Generic;
using SecretCommunicator.Models;
using SecretCommunicator.Models.Library;
using Spring.Social.Dropbox.Api;
using Spring.Social.Dropbox.Connect;
using Spring.Social.OAuth1;

namespace SecretCommunicator.Data
{
    public static class AppCache
    {
        public static IOAuth1ServiceProvider<IDropbox> dropboxProvider = new DropboxServiceProvider("pvpumiltdhmm2ge", "9oxuhy91gr1y4bs", AccessLevel.AppFolder);

        public static OAuthToken requestToken =
            dropboxProvider.OAuthOperations.FetchRequestTokenAsync(null, null).Result;
        public static OAuthToken token { get; set; }

        
        public static AESProvider AESProvider = new AESProvider();

        private static List<User> _UserList = new List<User>();
        public static List<User> UserList
        {
            get { return _UserList; }
            set { _UserList = value; }
        }

        private static List<Channel> _ChannelList = new List<Channel>();
        public static List<Channel> ChannelList
        {
            get { return _ChannelList; }
            set { _ChannelList = value; }
        }

        private static List<Message> _MessageList = new List<Message>();
        public static List<Message> MessageList
        {
            get { return _MessageList; }
            set { _MessageList = value; }
        }
    }
}
