// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using LoyaltyApi.Models;
// using LoyaltyApi.Repositories;
// using Swashbuckle.AspNetCore.SwaggerUI;
//
// namespace LoyaltyApi.Services
// {
//   public class PointsService(IPointsRepository pointsRepository) : IPointsService
//   {
//     public async Task<Points> CreatePointsAsync(Points points)
//     {
//       await pointsRepository.CreatePointsAsync(points);
//       return points;
//     }
//
//     public async Task<Points?> GetPointsRecordAsync(int customerId, int restaurantId, int transactionId)
//     {
//       return await pointsRepository.GetPointsRecordAsync(customerId, restaurantId, transactionId);
//     }
//
//     public async Task<int> GetTotalPointsAsync(int customerId, int restaurantId)
//     {
//       var sum = 0;
//       var list = await pointsRepository.GetTotalPointsAsync(customerId, restaurantId);
//       list.ForEach(x => sum += x.CreditPoints);
//       return sum;
//     }
//   }
// }