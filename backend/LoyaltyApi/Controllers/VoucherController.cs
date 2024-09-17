using System.Security.Claims;
using LoyaltyApi.Exceptions;
using LoyaltyApi.Models;
using LoyaltyApi.RequestModels;
using LoyaltyApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoyaltyApi.Controllers;

/// <summary>
///  Controller for managing voucher-related operations.
/// </summary>
[ApiController]
[Route("api")]
public class VoucherController(
    IVoucherService voucherService,
    ICreditPointsTransactionService pointsTransactionService,
    ILogger<VoucherController> logger) : ControllerBase
{
    /// <summary>
    /// Create a new voucher.
    /// </summary>
    /// <param name="voucherRequest">The voucher request details.</param>
    /// <returns>The created voucher.</returns>
    /// <response code="201">Voucher created successfully.</response>
    /// <response code="400">Invalid request body or parameters.</response>
    /// <response code="401">Unauthorized access.</response>
    /// <response code="500">Server error.</response>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/vouchers
    /// 
    ///     {
    ///          "points": 100
    ///     }
    ///     
    /// Sample response:
    ///
    ///     201 Created
    ///     {
    ///         "success": true,
    ///         "message": "Voucher created",
    ///         "data": {
    ///             "voucher": "ABC123"
    ///         }
    ///     }
    /// 
    /// Authorization header with JWT Bearer token is required.
    /// </remarks>
    [HttpPost]
    [Route("vouchers")]
    [Authorize(Roles = "User")]
    public async Task<ActionResult> CreateVoucher([FromBody] CreateVoucherRequest voucherRequest)
    {
        logger.LogInformation("Creating voucher for customer {CustomerId} and restaurant {RestaurantId}",
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value, User.FindFirst("RestaurantId")?.Value);
        try
        {
            string userClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                               throw new UnauthorizedAccessException("User id not found in token");
            string restaurantClaim = User.FindFirst("restaurantId")?.Value ??
                                     throw new UnauthorizedAccessException("Restaurant id not found in token");
            _ = int.TryParse(userClaim, out var userId);
            _ = int.TryParse(restaurantClaim, out var restaurantId);
            Voucher voucher = await voucherService.CreateVoucherAsync(voucherRequest, userId, restaurantId);
            await pointsTransactionService.SpendPointsAsync(voucher.CustomerId, voucher.RestaurantId,
                voucherRequest.Points);
            return StatusCode(StatusCodes.Status201Created,
                new
                {
                    success = true,
                    message = "Voucher created",
                    data = new { voucher = voucher.ShortCode }
                });
        }
        catch (PointsNotEnoughException ex)
        {
            logger.LogError(ex, "Points not enough for customer {CustomerId} and restaurant {RestaurantId}",
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value, User.FindFirst("RestaurantId")?.Value);
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (MinimumPointsNotReachedException ex)
        {
            logger.LogError(ex,
                "Minimum points not reached for customer {CustomerId} and restaurant {RestaurantId}",
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value, User.FindFirst("RestaurantId")?.Value);
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Create voucher failed for customer {CustomerId} and restaurant {RestaurantId}",
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value, User.FindFirst("RestaurantId")?.Value);
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Get a voucher by short code.
    /// </summary>
    /// <param name="shortCode">The short code of the voucher.</param>
    /// <returns>The voucher details.</returns>
    /// <response code="200">Voucher retrieved successfully.</response>
    /// <response code="401">Unauthorized access.</response>
    /// <response code="500">Server error.</response>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/vouchers
    ///     
    /// Sample response:
    ///
    ///     200 OK
    ///     {
    ///         "success": true,
    ///         "message": "Voucher created",
    ///         "data": [
    ///             {
    ///                 "shortCode": "ABC123",
    ///                 "isUsed": false,
    ///                 "value": 100
    ///             },
    ///             {
    ///                 "shortCode": "DEF456",
    ///                 "isUsed": true,
    ///                 "value": 200
    ///             }
    ///         ] 
    ///     }
    ///
    /// shortCode can be provided as query parameter for getting a specific voucher.
    /// Authorization header with JWT Bearer token is required.
    /// </remarks>
    [HttpGet]
    [Route("vouchers/{shortCode}")]
    [Authorize(Roles = "User")]
    public async Task<ActionResult> GetVoucherByShortCode(string shortCode)
    {
        logger.LogInformation("Getting voucher {ShortCode} for customer {CustomerId} and restaurant {RestaurantId}",
            shortCode, User.FindFirst(ClaimTypes.NameIdentifier)?.Value, User.FindFirst("RestaurantId")?.Value);
        try
        {
            string userClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                               throw new UnauthorizedAccessException();
            string restaurantClaim = User.FindFirst("restaurantId")?.Value ?? throw new UnauthorizedAccessException();
            _ = int.TryParse(userClaim, out var userId);
            _ = int.TryParse(restaurantClaim, out var restaurantId);
            var voucher = await voucherService.GetVoucherAsync(userId, restaurantId, shortCode);
            var result = new
            {
                voucher.ShortCode,
                voucher.Value,
                voucher.IsUsed
            };
            return Ok(new
            {
                success = true,
                message = "Voucher retrieved successfully",
                data = new { voucher = result }
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Get voucher failed for customer {CustomerId} and restaurant {RestaurantId}",
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value, User.FindFirst("RestaurantId")?.Value);
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    [Route("vouchers")]
    [Authorize(Roles = "User")]
    public async Task<ActionResult> GetVouchers([FromQuery] int pageNumber, [FromQuery] int pageSize)
    {
        try
        {
            string userClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                               throw new UnauthorizedAccessException();
            string restaurantClaim = User.FindFirst("restaurantId")?.Value ?? throw new UnauthorizedAccessException();
            _ = int.TryParse(userClaim, out var userId);
            _ = int.TryParse(restaurantClaim, out var restaurantId);
            var paginationResult =
                await voucherService.GetUserVouchersAsync(userId, restaurantId, pageNumber, pageSize);
            var vouchers = paginationResult.Vouchers;
            var vouchersResult = vouchers.Select(voucher => new
            {
                voucher.ShortCode,
                voucher.Value,
                voucher.IsUsed
            });
            var paginationMetadata = paginationResult.PaginationMetadata;
            return Ok(new
            {
                success = true,
                message = "Vouchers retrieved successfully",
                data = new { vouchersResult },
                metadata = paginationMetadata
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            logger.LogError(ex, "Get vouchers failed for customer {CustomerId} and restaurant {RestaurantId}",
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value, User.FindFirst("RestaurantId")?.Value);
            return StatusCode(401, new { success = false, message = ex.Message });
        }

        catch (Exception ex)
        {
            logger.LogError(ex, "Get vouchers failed for customer {CustomerId} and restaurant {RestaurantId}",
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value, User.FindFirst("RestaurantId")?.Value);
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Get a voucher by short code for admin purposes.
    /// </summary>
    /// <param name="shortCode">The short code of the voucher.</param>
    /// <param name="customerId">The customer ID associated with the voucher.</param>
    /// <param name="restaurantId">The restaurant ID associated with the voucher.</param>
    /// <returns>The long code of the voucher.</returns>
    /// <response code="200">Voucher retrieved successfully.</response>
    /// <response code="401">Unauthorized access.</response>
    /// <response code="500">Server error.</response>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /api/admin/vouchers?shortCode=ABC123&customerId=1&restaurantId=1
    ///
    /// Sample response:
    ///
    ///     200 OK
    ///     {
    ///         "success": true,
    ///         "message": "Voucher retrieved successfully",
    ///         "data": {
    ///             "voucher": "ABCDEFGHIJK1234567890"
    ///         }
    ///     }
    ///
    ///  **Admin Only Endpoint**
    /// Authorization header with JWT Bearer token is required.
    /// </remarks>
    [HttpGet]
    [Route("admin/vouchers")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> GetVoucherLongCode([FromQuery] string shortCode, [FromQuery] int customerId,
        [FromQuery] int restaurantId)
    {
        logger.LogInformation("Getting voucher {ShortCode} for customer {CustomerId} and restaurant {RestaurantId}",
            shortCode, customerId, restaurantId);
        try
        {
            Voucher voucher = await voucherService.GetVoucherAsync(customerId, restaurantId, shortCode);
            return Ok(new
            {
                success = true,
                message = "Voucher retrieved successfully",
                data = new { longCode = voucher.LongCode }
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Getting voucher with short code {ShortCode} failed for customer {CustomerId} and restaurant {RestaurantId}",
                shortCode, customerId, restaurantId);
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }
}