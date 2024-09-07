using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoyaltyApi.Controllers
{
    [ApiController]
    [Route("api/credit-points")]
    public class CreditPointsController(ICreditPointsTransactionService pointsTransactionService,
    ILogger<CreditPointsController> logger) : ControllerBase
    {
        [HttpGet]
        [Route("")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult> GetPoints()
        {
            logger.LogInformation("Get points request");
            int points = await pointsTransactionService.GetCustomerPointsAsync(null, null);
            return Ok(points);
        }

    }
}