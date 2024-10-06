using LoyaltyPointsApi.RequestModels;
using LoyaltyPointsApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace LoyaltyPointsApi.Controllers
{
    [ApiController]
    [Route("api/thresholds")]
    public class ThresholdController(IThresholdService thresholdService,
    ILogger<ThresholdController> logger) : ControllerBase
    {
        /// <summary>
        /// Add a new threshold
        /// </summary>
        /// <remarks>
        /// Adds a new threshold object to the database.
        /// </remarks>
        /// <param name="thresholdRequestModel">The threshold to add</param>
        /// <example>
        /// Request:
        /// POST /api/thresholds
        /// Content-Type: application/json
        /// {
        ///     "ThresholdName": "Silver",
        ///     "MinimumPoints": 10,
        ///     "RestaurantId": 1
        /// }
        /// Response:
        /// HTTP/1.1 201 Created
        /// Content-Type: application/json
        /// {
        ///     "success": true,
        ///     "message": "Threshold added"
        /// }
        /// </example>
        /// <returns>A 201 Created response with a success message</returns>
        /// <response code="201">Threshold added</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> AddThreshold([FromBody] ThresholdRequestModel thresholdRequestModel)
        {
            logger.LogInformation("Add Threshold request: {threshold} for restaurant: {restaurantId}", thresholdRequestModel.ThresholdName, thresholdRequestModel.RestaurantId);
            try
            {
                await thresholdService.AddThreshold(thresholdRequestModel);
                return StatusCode(201, new { success = true, message = "Threshold added" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get the thresholds for a restaurant
        /// </summary>
        /// <remarks>
        /// Gets the thresholds for a restaurant.
        /// </remarks>
        /// <param name="restaurantId">The restaurant id</param>
        /// <example>
        /// Request:
        /// GET /api/thresholds/1
        /// Response:
        /// HTTP/1.1 200 OK
        /// Content-Type: application/json
        ///
        /// {
        ///     "success": true,
        ///     "message": "Thresholds found",
        ///     "data": [
        ///     {
        ///         "thresholdId": 1,
        ///         "restaurantId": 1,
        ///         "thresholdName": "Bronze",
        ///         "minimumPoints": 100
        ///     },
        ///     {
        ///         "thresholdId": 2,
        ///         "restaurantId": 1,
        ///         "thresholdName": "Silver",
        ///         "minimumPoints": 500
        ///     }
        ///     ]
        /// }
        /// </example>
        /// <returns>A 200 OK response with a list of thresholds</returns>
        /// <response code="200">Thresholds found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Route("{restaurantId}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetRestaurantThresholds([FromRoute] int restaurantId)
        {
            logger.LogInformation("Get Thresholds request for restaurant: {restaurantId}", restaurantId);
            try
            {
                var result = await thresholdService.GetRestaurantThresholds(restaurantId);

                if (result == null) return NotFound(new { success = false, message = "Threshold not found" });

                var responseData = result.Select(t => new
                {
                    thresholdId = t.ThresholdId,
                    restaurantId = t.RestaurantId,
                    thresholdName = t.ThresholdName,
                    minimumPoints = t.MinimumPoints
                }).ToList();

                return Ok(new
                {
                    success = true,
                    message = "Thresholds found",
                    data = responseData
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get a specific threshold for a restaurant
        /// </summary>
        /// <param name="restaurantId">The restaurant id</param>
        /// <param name="thresholdId">The threshold id</param>
        /// <returns>The threshold found</returns>
        /// <response code="200">Threshold found</response>
        /// <response code="404">Threshold not found</response>
        /// <example>
        /// Request:
        /// GET /api/loyalty-points/restaurants/1/thresholds/1 HTTP/1.1
        /// Host: localhost:5000
        /// Content-Type: application/json
        /// </example>
        /// <example>
        /// Response:
        /// HTTP/1.1 200 OK
        /// Content-Type: application/json
        /// {
        ///     "success": true,
        ///     "message": "Threshold found",
        ///     "data": {
        ///         "thresholdId": 1,
        ///         "restaurantId": 1,
        ///         "thresholdName": "500 points",
        ///         "minimumPoints": 500
        ///     }
        /// }
        /// </example>
        [HttpGet]
        [Route("restaurants/{restaurantId}/thresholds/{thresholdId}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetRestaurantThreshold([FromRoute] int restaurantId,
            [FromRoute] int thresholdId)
        {
            logger.LogInformation("Get Threshold request: {threshold} for restaurant: {restaurantId}", thresholdId, restaurantId);
            try
            {
                var result = await thresholdService.GetRestaurantThreshold(restaurantId, thresholdId);
                if (result == null) return NotFound(new { success = false, message = "Threshold not found" });
                var responseData = new                                       
                {                                                                               
                    thresholdId = result.ThresholdId,                                                
                    restaurantId = result.RestaurantId,                                              
                    thresholdName = result.ThresholdName,                                            
                    minimumPoints = result.MinimumPoints                                             
                };       
                return Ok(new
                {
                    success = true,
                    message = "Threshold found",
                    data = responseData
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Update a threshold for a restaurant
        /// </summary>
        /// <param name="thresholdRequestModel">The updated threshold</param>
        /// <param name="restaurantId">The restaurant id</param>
        /// <param name="thresholdId">The threshold id</param>
        /// <returns>The updated threshold</returns>
        /// <response code="200">Threshold updated</response>
        /// <response code="404">Threshold not found</response>
        /// <example>
        /// Request:
        /// PUT /api/loyalty-points/restaurants/1/thresholds/1 HTTP/1.1
        /// Host: localhost:5000
        /// Content-Type: application/json
        /// {
        ///     "thresholdName": "1000 points",
        ///     "minimumPoints": 1000
        /// }
        /// </example>
        /// <example>
        /// Response:
        /// HTTP/1.1 200 OK
        /// Content-Type: application/json
        /// {
        ///     "success": true,
        ///     "message": "Threshold updated",
        ///     "data": {
        ///         "thresholdId": 1,
        ///         "restaurantId": 1,
        ///         "thresholdName": "1000 points",
        ///         "minimumPoints": 1000
        ///     }
        /// }
        /// </example>
        [HttpPut]
        [Route("restaurants/{restaurantId}/thresholds/{thresholdId}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateThreshold([FromBody] ThresholdRequestModel thresholdRequestModel,
            [FromRoute] int restaurantId, [FromRoute] int thresholdId)
        {
            logger.LogInformation("Update Threshold request: {threshold} for restaurant: {restaurantId}", thresholdId, restaurantId);
            try
            {
                var threshold = await thresholdService.GetRestaurantThreshold(restaurantId, thresholdId);
                if (threshold == null) return NotFound(new { success = false, message = "Threshold not found" });
                var result = await thresholdService.UpdateThreshold(thresholdRequestModel, restaurantId, thresholdId);
                return Ok(new
                {
                    success = true,
                    message = "Threshold updated",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Delete a threshold
        /// </summary>
        /// <param name="thresholdId">The threshold id to delete</param>
        /// <returns>A 200 OK response with a success message</returns>
        /// <response code="200">Threshold deleted</response>
        /// <response code="404">Threshold not found</response>
        /// <response code="500">Internal server error</response>
        /// <example>
        /// Request:
        /// DELETE /api/thresholds/1 HTTP/1.1
        /// Response:
        /// HTTP/1.1 200 OK
        /// Content-Type: application/json
        /// {
        ///     "success": true,
        ///     "message": "Threshold deleted",
        ///     "data": null
        /// }
        /// </example>
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        [HttpDelete]
        [Route("{thresholdId}")]
        public async Task<ActionResult> DeleteThreshold([FromRoute] int thresholdId)
        {
            logger.LogInformation("Delete Threshold request: {thresholdId}", thresholdId);
            try
            {
                await thresholdService.DeleteThreshold(thresholdId);
                return Ok(new { success = true, message = "Threshold deleted", });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}