using LoyaltyPointsApi.RequestModels;
using LoyaltyPointsApi.Services;
using Microsoft.AspNetCore.Mvc;
namespace LoyaltyPointsApi.Controllers

{
    [ApiController]
    [Route("api/promotions")]
    public class PromotionController(IPromotionService promotionService,
    ILogger<PromotionController> logger) : ControllerBase
    {
        /// <summary>
        /// Add a promotion to the database
        /// </summary>
        /// <param name="promotionRequestModel">The promotion to add</param>
        /// <returns>A 201 Created response with a success message</returns>
        /// <response code="201">Promotion added</response>
        /// <response code="500">Internal Server Error</response>
        /// <example>
        /// Request:
        /// POST /api/promotions HTTP/1.1
        /// Content-Type: application/json
        /// {
        ///     "promoCode": "SUMMER2022",
        ///     "restaurantId": 1,
        ///     "thresholdId": 1
        /// }
        /// Response:
        /// HTTP/1.1 201 Created
        /// Content-Type: application/json
        /// {
        ///     "success": true,
        ///     "message": "Promotion added"
        /// }
        /// </example>
        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> AddPromotion([FromBody] AddPromotionRequestModel promotionRequestModel)
        {
            logger.LogInformation("Add Promotion request: {promotion} for restaurant: {restaurantId}", promotionRequestModel.PromoCode, promotionRequestModel.RestaurantId);
            try
            {
                await promotionService.AddPromotion(promotionRequestModel);
                return StatusCode(StatusCodes.Status201Created, new { success = true, message = "Promotion added" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Update a promotion in the database
        /// </summary>
        /// <param name="promoCode">The promo code of the promotion to update</param>
        /// <param name="restaurantId">The restaurant id of the promotion to update</param>
        /// <param name="promotionRequestModel">The new promotion data</param>
        /// <returns>A 200 OK response with a success message</returns>
        /// <response code="200">Promotion updated</response>
        /// <response code="404">Promotion not found</response>
        /// <example>
        /// Request:
        /// PUT /api/promotions/SUMMER2022/restaurants/1 HTTP/1.1
        /// Content-Type: application/json
        /// {
        ///     "thresholdId": 2
        /// }
        /// Response:
        /// HTTP/1.1 200 OK
        /// Content-Type: application/json
        /// {
        ///     "success": true,
        ///     "message": "Promotion updated"
        /// }
        /// </example>
        [HttpPut]
        [Route("{promoCode}/restaurants/{restaurantId}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdatePromotion([FromRoute] string promoCode, [FromRoute] int restaurantId, [FromBody] UpdatePromotionRequestModel promotionRequestModel)
        {
            logger.LogInformation("Update Promotion request: {promotion} for restaurant: {restaurantId}", promoCode, restaurantId);
            try
            {
                var result = await promotionService.UpdatePromotion(promoCode, promotionRequestModel, restaurantId);
                if (result == null) return NotFound(new { success = false, message = "Promotion not found" });
                return Ok(new { success = true, message = "Promotion updated" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get all promotions for a restaurant
        /// </summary>
        /// <param name="restaurantId">The restaurant id</param>
        /// <returns>A 200 OK response with a list of promotions</returns>
        /// <response code="200">Promotions found</response>
        /// <response code="404">Promotions not found</response>
        /// <example>
        /// Request:
        /// GET /api/promotions/restaurants/1 HTTP/1.1
        /// Response:
        /// HTTP/1.1 200 OK
        /// Content-Type: application/json
        /// {
        ///     "success": true,
        ///     "message": "Promotions found",
        ///     "data": [
        ///     {
        ///         "restaurantId": 1,
        ///         "promoCode": "save200",
        ///         "thresholdId": 1,
        ///         "isNotified": false,
        ///         "startDate": "0001-01-01T00:00:00",
        ///         "endDate": "0001-01-01T00:00:00",
        ///         "threshold": null
        ///     },
        ///     {
        ///         "restaurantId": 1,
        ///         "promoCode": "save100",
        ///         "thresholdId": 1,
        ///         "isNotified": false,
        ///         "startDate": "0001-01-01T00:00:00",
        ///         "endDate": "0001-01-01T00:00:00",
        ///         "threshold": null
        ///     }
        ///     ],
        ///     "metadata": {
        ///         "pageNumber": 1,
        ///         "pageSize": 2,
        ///         "totalItemCount": 4,
        ///         "pageCount": 2,
        ///         "hasPreviousPage": false,
        ///         "hasNextPage": true
        ///     }
        /// }
        /// </example>
        [HttpGet]
        [Route("restaurants/{restaurantId}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetRestaurantPromotions([FromRoute] int restaurantId,[FromQuery] int pageNumber = 1,[FromQuery] int pageSize = 10)
        {
            logger.LogInformation("Getting promotions request for restaurant: {restaurantId}", restaurantId);
            try
            {
                var pagedPromotions = await promotionService.GetThresholdPromotions(restaurantId,  pageNumber,  pageSize);
                if (pagedPromotions == null) return NotFound(new { success = false, message = "Promotions not found" });
                return Ok(new
                {
                    success = true,
                    message = "Promotions found",
                    data = pagedPromotions.ToList(),
                    metadata = new
                    {
                        PageNumber = pagedPromotions.PageNumber,
                        PageSize = pagedPromotions.PageSize,
                        TotalItemCount = pagedPromotions.TotalItemCount,
                        PageCount = pagedPromotions.PageCount,
                        HasPreviousPage = pagedPromotions.HasPreviousPage,
                        HasNextPage = pagedPromotions.HasNextPage
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get a promotion by promo code
        /// </summary>
        /// <param name="promoCode">The promo code</param>
        /// <returns>A 200 OK response with the promotion</returns>
        /// <response code="200">Promotion found</response>
        /// <response code="404">Promotion not found</response>
        /// <example>
        /// Request:
        /// GET /api/promotions/SUMMER2022 HTTP/1.1
        /// Response:
        /// HTTP/1.1 200 OK
        /// Content-Type: application/json
        /// {
        ///     "success": true,
        ///     "message": "Promotion found",
        ///     "data": {
        ///         "id": 1,
        ///         "promoCode": "SUMMER2022",
        ///         "restaurantId": 1,
        ///         "thresholdId": 1
        ///     }
        /// }
        /// </example>
        [HttpGet]
        [Route("{promoCode}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetPromotion([FromRoute] string promoCode)
        {
            logger.LogInformation("Get Promotion Request: {promoCode}", promoCode);
            try
            {
                var result = await promotionService.GetPromotion(promoCode);
                if (result == null) return NotFound(new { success = false, message = "Promotion not found" });
                return Ok(new
                {
                    success = true,
                    message = "Promotion found",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        /// <summary>
        /// Delete a promotion by promo code
        /// </summary>
        /// <param name="promoCode">The promo code</param>
        /// <returns>A 200 OK response with a success message</returns>
        /// <response code="200">Promotion deleted</response>
        /// <response code="404">Promotion not found</response>
        /// <example>
        /// Request:
        /// DELETE /api/promotions/SUMMER2022 HTTP/1.1
        /// Response:
        /// HTTP/1.1 200 OK
        /// Content-Type: application/json
        /// {
        ///     "success": true,
        ///     "message": "Promotion deleted"
        /// }
        /// </example>
        [HttpDelete]
        [Route("{promoCode}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeletePromotion([FromRoute] string promoCode)
        {
            logger.LogInformation("Delete Promotion Request: {promoCode}", promoCode);
            try
            {
                await promotionService.DeletePromotion(promoCode);
                return Ok(new { success = true, message = "Promotion deleted" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}