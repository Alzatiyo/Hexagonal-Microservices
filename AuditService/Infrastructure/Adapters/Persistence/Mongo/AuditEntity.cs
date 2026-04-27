using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Infrastructure.Adapters.Persistence.Mongo
{
    public class AuditEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        [BsonElement("_id")]
        public string Id { get; set; } = string.Empty;

        [BsonElement("Action")]
        public string Action { get; set; } = string.Empty;

        [BsonElement("EntityName")]
        public string EntityName { get; set; } = string.Empty;

        [BsonElement("User")]
        public string User { get; set; } = string.Empty;

        [BsonElement("date")]
        public DateTime Date { get; set; }

        [BsonElement("Details")]
        public string Details { get; set; } = string.Empty;
    }
}