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
    ///     GET /api/vouchers/ABC123
    ///     
    /// Sample response:
    ///
    ///     200 OK
    ///     {
    ///         "success": true,
    ///         "message": "Voucher retrieved successfully",
    ///         "data": 
    ///         {
    ///             "voucher": 
    ///             {
    ///                 "shortCode": "ABC123",
    ///                 "value": 100
    ///                 "isUsed": false
    ///             }
    ///         }
    ///     }
    ///
    /// shortCode can be provided as query parameter for getting a specific voucher.
    /// Authorization header with JWT Bearer token is required.
    /// </remarks>
    [HttpGet]
    [Route("vouchers/{shortCode}")]
    [Authorize(Roles = "User")]
    public async Task<ActionResult> GetUserVoucherByShortCode(string shortCode)
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

    /// <summary>
    /// Gets all vouchers for the current user.
    /// </summary>
    /// <param name="pageNumber">The page number.</param>
    /// <param name="pageSize">The page size.</param>
    /// <returns>The vouchers.</returns>
    /// <response code="200">Vouchers retrieved successfully.</response>
    /// <response code="401">Unauthorized access.</response>
    /// <response code="500">Server error.</response>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /vouchers?pageNumber=1&pageSize=10
    ///
    /// Sample response:
    ///
    ///     200 OK
    ///     {
    ///         "success": true,
    ///         "message": "Vouchers retrieved successfully",
    ///         "data": {
    ///             "vouchers": [
    ///                 {
    ///                     "shortCode": "ABCD",
    ///                     "value": 100,
    ///                     "isUsed": false
    ///                 },
    ///                 {
    ///                     "shortCode": "EFGH",
    ///                     "value": 200,
    ///                     "isUsed": true
    ///                 }
    ///             ],
    ///             "metadata": {
    ///                 "totalCount": 10,
    ///                 "totalPages": 1,
    ///                 "pageSize": 10,
    ///                 "pageNumber": 1
    ///             }
    ///         }
    ///     }
    ///
    /// Authorization header with JWT Bearer token is required.
    /// </remarks>
    [HttpGet]
    [Route("vouchers")]
    [Authorize(Roles = "User")]
    public async Task<ActionResult> GetVouchersByJwtToken([FromQuery] int pageNumber, [FromQuery] int pageSize)
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
    ///     GET admin/restaurants/600/users/7/vouchers/ABCDE
    ///
    /// Sample response:
    ///
    ///     200 OK
    ///     {
    ///         "success": true,
    ///         "message": "Voucher retrieved successfully",
    ///         "data": {
    ///             "voucher":
    ///             {
    ///                 "shortCode": "ABCDE",
    ///                 "longCode": "1234567890",
    ///                 "value": 100,
    ///                 "isUsed": false
    ///             }
    ///         }
    ///     }
    ///
    ///  **Admin Only Endpoint**
    /// Authorization header with JWT Bearer token is required.
    /// </remarks>
    [HttpGet]
    [Route("admin/vouchers/{shortCode}/restaurants/{restaurantId}/users/{customerId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> GetVoucherLongCode(int restaurantId, int customerId, string shortCode)
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
                data = new
                {
                    voucher = new
                    {
                        voucher.ShortCode,
                        voucher.LongCode,
                        voucher.Value,
                        voucher.IsUsed
                    }
                }
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
    /// <summary>
    /// Sets a voucher iUsed.
    /// </summary>
    /// <param name="shortCode">The short code of the voucher.</param>
    /// <param name="requestModel">The request model containing the value to set the voucher to used to.</param>
    /// <returns>An ActionResult indicating the result of the operation.</returns>
    /// <response code="200">If the voucher is set to used successfully.</response>
    /// <response code="401">If the user is not authorized.</response>
    /// <response code="500">If any other exception occurs.</response>
    /// <remarks>
    /// Sample request:
    ///
    ///     PUT /api/vouchers/ABCDE
    ///     {
    ///         "isUsed": true
    ///     }
    ///
    /// Sample response:
    ///     200 OK
    ///     {
    ///         "success": true,
    ///         "message": "Voucher isUsed set to true",
    ///         "data": {
    ///             "voucher": {
    ///                 "shortCode": "ABCDE",
    ///                 "longCode": "1234567890",
    ///                 "value": 100,
    ///                 "isUsed": true
    ///             }
    ///         }
    ///     }
    ///
    /// </remarks>
    [HttpPut]
    [Route("admin/vouchers/{shortCode}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> SetIsUsed([FromRoute] string shortCode, [FromBody] SetIsUsedRequestModel requestModel)
    {
        logger.LogInformation("Setting voucher {ShortCode} to used", shortCode);
        try
        {

            Voucher voucher = await voucherService.SetIsUsedAsync(shortCode, requestModel);
            return Ok(new
            {
                success = true,
                message = $"Voucher isUsed set to {requestModel.IsUsed}",
                data = new
                {
                    voucher = new
                    {
                        voucher.ShortCode,
                        voucher.LongCode,
                        voucher.Value,
                        voucher.IsUsed
                    }
                }
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Setting voucher {ShortCode} to used failed", shortCode);
            return StatusCode(500, new
            {
                success = false,
                message = ex.Message
            });
        }
    }
    /// <summary>
    /// Gets all vouchers for a customer with a specific restaurant ID.
    /// </summary>
    /// <param name="restaurantId">The restaurant ID.</param>
    /// <param name="customerId">The customer ID.</param>
    /// <param name="pageNumber">The page number.</param>
    /// <param name="pageSize">The page size.</param>
    /// <returns>The vouchers.</returns>
    /// <response code="200">Vouchers retrieved successfully.</response>
    /// <response code="401">Unauthorized access.</response>
    /// <response code="500">Server error.</response>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /api/vouchers/admin/vouchers/restaurants/1/users/2?pageNumber=1&pageSize=10
    ///
    /// Sample response:
    ///
    ///     200 OK
    ///     {
    ///         "success": true,
    ///         "message": "Vouchers retrieved successfully",
    ///         "data": {
    ///             "vouchers": [
    ///                 {
    ///                     "shortCode": "ABCD",
    ///                     "value": 100,
    ///                     "isUsed": false
    ///                 },
    ///                 {
    ///                     "shortCode": "EFGH",
    ///                     "value": 200,
    ///                     "isUsed": true
    ///                 }
    ///             ],
    ///             "metadata": {
    ///                 "totalCount": 25,
    ///                 "totalPages": 3,
    ///                 "pageSize": 2,
    ///                 "pageNumber": 1
    ///             }
    ///         }
    ///     }
    ///
    /// </remarks>
    [HttpGet]
    [Route("admin/vouchers/restaurants/{restaurantId}/users/{customerId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> GetUserVouchersByRestaurantIdAndCustomerId([FromRoute] int restaurantId, [FromRoute] int customerId, [FromQuery] int pageNumber, [FromQuery] int pageSize)
    {
        logger.LogInformation("Getting vouchers for customer {CustomerId}", customerId);
        try
        {
            PagedVouchersResponse paginationResult = await voucherService.GetUserVouchersAsync(customerId, restaurantId, pageNumber, pageSize);
            var vouchers = paginationResult.Vouchers.Select(v => new
            {
                v.ShortCode,
                v.LongCode,
                v.Value,
                v.IsUsed
            }).ToList();
            var metadata = paginationResult.PaginationMetadata;
            return Ok(new
            {
                success = true,
                message = "Vouchers retrieved successfully",
                data = new
                {
                    vouchers
                },
                metadata
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Getting vouchers for customer {CustomerId} failed", customerId);
            return StatusCode(500, new
            {
                success = false,
                message = ex.Message
            });
        }
    }

}