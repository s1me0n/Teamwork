using System;
using System.Collections.Generic;

namespace SecretCommunicator.WebData.Library
{
    public class User
    {

        public Guid Id
        {
            get;
            set;
        }

        public List<string> ChannelName { get; set; }

    }
}
