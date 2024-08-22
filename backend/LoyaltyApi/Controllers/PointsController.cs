using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyApi.Models;
using LoyaltyApi.RequestModels;
using LoyaltyApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace LoyaltyApi.Controllers
{
    [ApiController]
    [Route("api/points/")]
    public class PointsController(IPointsService pointsService) : ControllerBase
    {
        [HttpPost]
        [Route("")]
        public async Task<ActionResult> CreatePoints([FromBody] CreatePointsRequestModel requestBody)
        {
            try
            {
                Points points = new()
                {
                    CustomerId = requestBody.CustomerId,
                    RestaurantId = requestBody.RestaurantId,
                    TransactionId = requestBody.TransactionId,
                    CreditPoints = requestBody.CreditPoints,
                    DateOfCreation = DateTime.Now
                };
                await pointsService.CreatePointsAsync(points);
                return Ok("Points added");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("")]
        public async Task<ActionResult> GetTotalPoints([FromQuery] int customerId, [FromQuery]int restaurantId)
        {
            try
            {
                var result = await pointsService.GetTotalPointsAsync(customerId, restaurantId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}