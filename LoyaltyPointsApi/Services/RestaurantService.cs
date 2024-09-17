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

                RestaurantId = restaurantRequestModel.RestaurantId,

                PointsRate = restaurantRequestModel.PointsRate,

                ThresholdsNumber = restaurantRequestModel.ThresholdsNumber,

                PointsLifeTime = restaurantRequestModel.PointsLifeTime
            };

           await restaurantRepository.AddRestaurantSettings(restaurant);
            
        }

        public async Task<RestaurantSettings?> GetRestaurant(int ResturantId)
        {
            var restaurant = await restaurantRepository.GetRestaurant(ResturantId);
            return restaurant;
        }

        public async Task UpdateRestaurant(int restaurantId, UpdateRestaurantRequestModel updateRestaurantRequestModel)
        {
            var restaurant = await restaurantRepository.GetRestaurant(restaurantId);
            restaurant.PointsRate = updateRestaurantRequestModel.PointsRate;
            restaurant.PointsLifeTime = updateRestaurantRequestModel.PointsLifeTime;
            restaurant.ThresholdsNumber = updateRestaurantRequestModel.ThresholdsNumber;
            await restaurantRepository.UpdateRestaurant(restaurant);
        }
    }
}