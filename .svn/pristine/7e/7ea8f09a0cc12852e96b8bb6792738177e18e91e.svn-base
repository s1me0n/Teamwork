using System.Runtime.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using SecretCommunicator.Models.Interfaces;

namespace SecretCommunicator.Models.Library
{
    [DataContract]
    public class Message : IIdentifier
    {
        [DataMember(Name = "Id")]
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))] 
        public string Id { get; set; }
        [DataMember]
        public string ChannelId { get; set; }
        [DataMember]
        public MessageTypes Type { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Content { get; set; }
        [DataMember(Name = "CreatedDateTime")]
        public string CreatedDateTime { get; set; }
        [DataMember]
        [BsonIgnore]
        public string Status { get; set; }
    }

    public enum MessageTypes
    {
        Text = 1,
        Link = 2,
        File = 3,
        Image = 4
    }
}
