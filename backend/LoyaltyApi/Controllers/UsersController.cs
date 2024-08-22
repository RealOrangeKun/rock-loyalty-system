using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyApi.Models;
using LoyaltyApi.Repositories;
using LoyaltyApi.RequestModels;
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
        public async Task<ActionResult> CreateUser([FromBody] RegisterRequestBody requestBody)
        {
            try
            {
                User user = new()
                {
                    PhoneNumber = requestBody.PhoneNumber,
                    Email = requestBody.Email,
                    Password = requestBody.Password,
                    RestaurantId = requestBody.RestaurantId,
                    Name = requestBody.Name

                };
                await userService.CreateUserAsync(user);
                return Ok("User created");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
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