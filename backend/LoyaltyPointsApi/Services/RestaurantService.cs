using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyPointsApi.Models;
using LoyaltyPointsApi.Repositories;
using LoyaltyPointsApi.RequestModels;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LoyaltyPointsApi.Services
{
    public class RestaurantService(IRestaurantRepository restaurantRepository) : IRestaurantService
    {
        public async Task AddRestaurantSettings(RestaurantRequestModel restaurantRequestModel)
        {
            RestaurantSettings restaurant = new (){
                Name = restaurantRequestModel.Name,

                RestaurantId = restaurantRequestModel.RestaurantId,

                PointsRate = restaurantRequestModel.PointsRate,

                ThresholdsNumber = restaurantRequestModel.ThresholdsNumber,

                PointsLifeTime = restaurantRequestModel.PointsLifeTime
            };

           await restaurantRepository.AddRestaurantSettings(restaurant);
            
        }

        public async Task<RestaurantSettings?> GetRestaurant(int ResturantId)
        {
            RestaurantSettings restaurant = new (){
                RestaurantId = ResturantId
            };
            return await restaurantRepository.GetRestaurant(restaurant);
        }

        public async Task<RestaurantSettings?> UpdateRestaurant(int restaurantId, UpdateRestaurantRequestModel updateRestaurantRequestModel)
        {
            RestaurantSettings restaurant = new(){
                RestaurantId = restaurantId
            };
            var result = await restaurantRepository.GetRestaurant(restaurant);
            result.PointsRate = updateRestaurantRequestModel.PointsRate;
            result.PointsLifeTime = updateRestaurantRequestModel.PointsLifeTime;
            result.ThresholdsNumber = updateRestaurantRequestModel.ThresholdsNumber;
            await restaurantRepository.UpdateRestaurant(result);
            return result;
        }
    }
}