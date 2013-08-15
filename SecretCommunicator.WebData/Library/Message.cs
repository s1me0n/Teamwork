using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using Newtonsoft.Json;
using System.IO;

namespace SecretCommunicator.WebData.Library
{
    [DataContract]
    public class Message
    {
        [DataMember(Name = "Id")]
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))] 
        public string Id { get; set; }

        public string ChannelId { get; set; }

        public dynamic PrivateData { get; set; }

        public DateTime PrivateDateTime { get; set; }

        [BsonIgnore]
        private string _CreatedDateTime = string.Empty;
        [BsonIgnore]
        [DataMember(Name = "CreatedDateTime")]
        public string CreatedDateTime
        {
            get 
            {
                if(PrivateDateTime != null)
                    _CreatedDateTime = PrivateDateTime.ToString();
                return _CreatedDateTime;
            }
            set { _CreatedDateTime = value; }
        }

        [BsonIgnore]
        private MessageResource _PublicData = new MessageResource();
        [BsonIgnore]
        [DataMember(Name = "PublicData")]
        public MessageResource PublicData
        {
            get 
            {
                if (!string.IsNullOrEmpty(PrivateData))
                {
                    dynamic pd = (new JsonSerializer()).Deserialize(new JsonTextReader(new StringReader(AppCache.AESProvider.DecryptString(PrivateData))));
                    _PublicData = new MessageResource() { Type = pd.Type, Value = pd.Value };
                }
                return _PublicData; 
            }
            set 
            { 
                _PublicData = value; 
            }
        }
    }

    public enum MessageTypes
    {
        Text,
        Link,
        File,
        Image
    }
}
