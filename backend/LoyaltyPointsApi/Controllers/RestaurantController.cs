using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyPointsApi.Models;
using LoyaltyPointsApi.RequestModels;
using LoyaltyPointsApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace LoyaltyPointsApi.Controllers
{
    [ApiController]
    [Route("api/restaurants")]
    public class RestaurantController(IRestaurantService restaurantService,
    ILogger<RestaurantController> logger) : ControllerBase
    {
        /// <summary>
        /// Add restaurant settings
        /// </summary>
        /// <remarks>
        /// Adds a new restaurant settings object to the database.
        /// </remarks>
        /// <param name="restaurantRequestModel">The restaurant settings to add</param>
        /// <example>
        /// Request:
        /// POST /api/restaurants
        /// Content-Type: application/json
        /// {
        ///     "Name": "KFC",
        ///     "RestaurantId": 1,
        ///     "PointsRate": 1,
        ///     "ThresholdsNumber": 1,
        ///     "PointsLifeTime": 1
        /// }
        /// Response:
        /// HTTP/1.1 201 Created
        /// Content-Type: application/json
        /// {
        ///     "success": true,
        ///     "message": "RestaurantSettings added"
        /// }
        /// </example>
        /// <returns>A 201 Created response with a success message</returns>
        /// <response code="201">RestaurantSettings added</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> AddRestaurantSettings([FromBody] RestaurantRequestModel restaurantRequestModel)
        {
            logger.LogInformation("Add Restaurant request: {restaurantId}", restaurantRequestModel.RestaurantId);
            try
            {
                await restaurantService.AddRestaurantSettings(restaurantRequestModel);
                return StatusCode(StatusCodes.Status201Created, new { success = true, message = "RestaurantSettings added" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Update restaurant settings
        /// </summary>
        /// <remarks>
        /// Updates an existing restaurant settings object in the database.
        /// </remarks>
        /// <param name="restaurantId">The restaurant id of the restaurant to update</param>
        /// <param name="updateRestaurantRequestModel">The new restaurant settings</param>
        /// <example>
        /// Request:
        /// PUT /api/restaurants/1
        /// Content-Type: application/json
        /// {
        ///     "Name": "Pizza Hut",
        ///     "PointsRate": 2,
        ///     "ThresholdsNumber": 2,
        ///     "PointsLifeTime": 2
        /// }
        /// Response:
        /// HTTP/1.1 200 OK
        /// Content-Type: application/json
        /// {
        ///     "success": true,
        ///     "message": "RestaurantSettings updated"
        /// }
        /// </example>
        /// <returns>A 200 OK response with a success message</returns>
        /// <response code="200">RestaurantSettings updated</response>
        /// <response code="404">Restaurant not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPut]
        [Route("{restaurantId}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateRestaurant([FromRoute] int restaurantId, [FromBody] UpdateRestaurantRequestModel updateRestaurantRequestModel)
        {
            logger.LogInformation("Update Restaurant request: {restaurantId}", restaurantId);
            try
            {
                var result = await restaurantService.GetRestaurant(restaurantId);
                if (result == null) return NotFound(new { success = false, message = "Restaurant not found" });

                await restaurantService.UpdateRestaurant(restaurantId, updateRestaurantRequestModel);

                return Ok(new { success = true, message = "RestaurantSettings updated" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get restaurant settings
        /// </summary>
        /// <remarks>
        /// Gets a restaurant settings object by restaurant id.
        /// </remarks>
        /// <param name="restaurantId">The restaurant id to get the settings for</param>
        /// <example>
        /// Request:
        /// GET /api/restaurants/1
        /// Response:
        /// HTTP/1.1 200 OK
        /// Content-Type: application/json
        /// {
        ///     "success": true,
        ///     "message": "Restaurant found",
        ///     "data": {
        ///         "Name": "KFC",
        ///         "RestaurantId": 1,
        ///         "PointsRate": 1,
        ///         "ThresholdsNumber": 1,
        ///         "PointsLifeTime": 1
        ///     }
        /// }
        /// </example>
        /// <returns>A 200 OK response with the restaurant settings</returns>
        /// <response code="200">Restaurant found</response>
        /// <response code="404">Restaurant not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Route("{restaurantId}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetRestaurant([FromRoute] int restaurantId)
        {
            logger.LogInformation("Get Restaurant request: {restaurantId}", restaurantId);
            try
            {
                var restaurant = await restaurantService.GetRestaurant(restaurantId);
                if (restaurant == null) return NotFound(new { success = false, message = "Restaurant not found" });
                return Ok(new
                {
                    success = true,
                    message = "Restaurant found",
                    data = new
                    {
                        restaurant
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

    }
}