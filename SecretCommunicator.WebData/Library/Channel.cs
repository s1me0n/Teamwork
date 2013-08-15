using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System.IO;
using MongoDB.Bson.Serialization.IdGenerators;

namespace SecretCommunicator.WebData.Library
{
    [DataContract]
    public class Channel
    {
        [DataMember(Name = "Id")]
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string Id { get; set; }

        [DataMember(Name = "Name")]
        public string Name { get; set; }

        [BsonIgnore]
        private string _CreatedDateTime = string.Empty;
        [BsonIgnore]
        [DataMember(Name = "CreatedDateTime")]
        public string CreatedDateTime
        {
            get
            {
                if (PublicData != null && PublicData.CreatedDateTime != null)
                    _CreatedDateTime = PublicData.CreatedDateTime.ToString();
                return _CreatedDateTime;
            }
            set { _CreatedDateTime = value; }
        }

        public dynamic PrivateData { get; set; }

        [BsonIgnore]
        private ChannelResource _PublicData = new ChannelResource();
        [BsonIgnore]
        public ChannelResource PublicData
        {
            get
            {
                if (!string.IsNullOrEmpty(PrivateData))
                {
                    dynamic pd = (new JsonSerializer()).Deserialize(new JsonTextReader(new StringReader(AppCache.AESProvider.DecryptString(PrivateData))));
                    _PublicData = new ChannelResource() { Password = pd.Password, CreatedDateTime = pd.CreatedDateTime };
                }
                return _PublicData;
            }
            set
            {
                _PublicData = value;
            }
        }
    }
}
