using LogiTrack.Models;
using Microsoft.AspNetCore.Mvc;

namespace LogiTrack.Controllers.Interfaces
{
    public interface IOrderService
    {
        Task<IActionResult> GetOrdersAsync();
        Task<IActionResult> GetOrderByIdAsync(int id);
        Task<IActionResult> CreateOrderAsync([FromBody] Order order);
        Task<IActionResult> DeleteOrderByIdAsync(int id);
    }
}