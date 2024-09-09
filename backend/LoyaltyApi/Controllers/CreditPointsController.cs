using LoyaltyApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoyaltyApi.Controllers;

/// <summary>
/// Controller for handling credit points.
/// </summary>
[ApiController]
[Route("api/credit-points")]
public class CreditPointsController(
    ICreditPointsTransactionService pointsTransactionService,
    ILogger<CreditPointsController> logger) : ControllerBase
{
    /// <summary>
    /// Retrieves the credit points for the authenticated user.
    /// </summary>
    /// <returns>Returns the credit points of the user.</returns>
    /// <response code="200">Returns the credit points of the user.</response>
    /// <response code="401">If the user is not authorized.</response>
    /// <response code="500">If there is an internal server error.</response>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /api/credit-points
    ///
    /// Sample response:
    ///
    ///     200 OK
    ///     {
    ///         "success": true,
    ///         "data": {
    ///             "points": 100
    ///         },
    ///         "message": "Points retrieved successfully"
    ///     }
    ///
    /// Authorization header with JWT Bearer token is required.
    /// </remarks>
    [HttpGet]
    [Route("")]
    [Authorize(Roles = "User")]
    public async Task<ActionResult> GetPoints()
    {
        try
        {
            logger.LogInformation("Get points request");
            var points = await pointsTransactionService.GetCustomerPointsAsync(null, null);
            return Ok(new { success = true, data = new { points }, message = "Points retrieved successfully" });
        }
        catch (UnauthorizedAccessException ex)
        {
            logger.LogError(ex, "Unauthorized access");
            return Unauthorized(new { success = false, message = "Unauthorized access" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while retrieving points");
            return StatusCode(500, new { success = false, message = "An error occurred while retrieving points" });
        }
    }
}