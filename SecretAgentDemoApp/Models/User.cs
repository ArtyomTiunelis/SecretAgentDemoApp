using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PromoApp.Api.Models;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("user_id")]
    public string UserId { get; set; } = string.Empty;

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("is_new_user")]
    public bool IsNewUser { get; set; }

    [BsonElement("promo_history")]
    public List<string>? PromoHistory { get; set; }

    [BsonElement("status")]
    public string Status { get; set; } = "active";
}
