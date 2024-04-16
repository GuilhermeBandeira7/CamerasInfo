using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CamerasInfo
{
    public class Ping_MongoDB
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonElement("AvailabilityConfig")]
        public int AvailabilityConfig { get; set; }
        [BsonElement("Counter")]
        public long Counter { get; set; }
        [BsonElement("DateTime")]
        public DateTime DateTime { get; set; } = DateTime.Now;
        [BsonElement("Status")]
        public string Status { get; set; } = string.Empty;
    }
}
