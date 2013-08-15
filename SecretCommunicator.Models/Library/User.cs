using System;
using System.Collections.Generic;
using SecretCommunicator.Models.Interfaces;

namespace SecretCommunicator.Models.Library
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
