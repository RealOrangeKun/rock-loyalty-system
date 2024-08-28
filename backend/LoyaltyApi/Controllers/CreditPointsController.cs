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
    public class CreditPointsController(ICreditPointsTransactionService pointsTransactionService) : ControllerBase
    {
        [HttpGet]
        [Route("")]
        [Authorize(Roles = "User, Admin")]
        public async Task<ActionResult> GetPoints()
        {
            int points = await pointsTransactionService.GetCustomerPointsAsync(null, null);
            return Ok(points);
        }

    }
}