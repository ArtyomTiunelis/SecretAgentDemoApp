using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using PromoApp.Api.Models;
using PromoApp.Api.Services;

namespace PromoApp.Api.Controllers;

[ApiController]
[Route("api/v1/promos")]
public class PromoController(MongoDbService mongoDbService) : ControllerBase
{
    [HttpPost("redeem")]
    public async Task<IActionResult> RedeemPromo(PromoRedeemRequest request)
    {
        HttpContext.Items["UserId"] = request.UserId;
        HttpContext.Items["TicketId"] = request.TicketId;

        var user = await mongoDbService.Users
            .Find(candidate => candidate.UserId == request.UserId)
            .FirstOrDefaultAsync();

        if (user is null)
        {
            return NotFound(new { error = "User not found" });
        }

        // INTENTIONAL BUG: Unhandled NullReferenceException when PromoHistory is null on new accounts
        if (user.PromoHistory.Contains(request.Code))
        {
            return BadRequest(new { error = "Promo code already redeemed" });
        }

        user.PromoHistory.Add(request.Code);

        await mongoDbService.Users.ReplaceOneAsync(candidate => candidate.Id == user.Id, user);

        return Ok(new
        {
            message = "Promo code redeemed successfully",
            userId = user.UserId,
            code = request.Code
        });
    }
}
