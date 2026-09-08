namespace PromoApp.Api.Models;

public class PromoRedeemRequest
{
    public string UserId { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string? TicketId { get; set; }
}
