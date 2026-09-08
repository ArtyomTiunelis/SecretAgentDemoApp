using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PromoApp.Api.Models;

public class ErrorLog
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("ticket_id")]
    public string? TicketId { get; set; }

    [BsonElement("user_id")]
    public string? UserId { get; set; }

    [BsonElement("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [BsonElement("endpoint")]
    public string Endpoint { get; set; } = string.Empty;

    [BsonElement("status_code")]
    public int StatusCode { get; set; }

    [BsonElement("message")]
    public string Message { get; set; } = string.Empty;
}
