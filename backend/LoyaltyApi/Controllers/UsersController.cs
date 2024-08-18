using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyApi.Models;
using LoyaltyApi.Repositories;
using LoyaltyApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace LoyaltyApi.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UsersController(IUserService userService) : ControllerBase
    {
        [HttpPost]
        [Route("")]
        public async Task<ActionResult> CreateUser([FromBody] User user)
        {
            try
            {
                await userService.CreateUserAsync(user);
                return Ok("User created");
            }
            catch (HttpRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }


        }
    }
}