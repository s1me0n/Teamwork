using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace SecretCommunicator.WebData.Library
{
    public class ChannelResource
    {
        public string Password { get; set; }

        public DateTime CreatedDateTime { get; set; }
    }
}
