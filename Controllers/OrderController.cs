using LogiTrack.Controllers.Interfaces;
using LogiTrack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace LogiTrack.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase, IOrderService
    {
        private readonly LogiTrackContext dbContext;
        private readonly IMemoryCache _cache;

        private const string CacheKey = "Orders";

        public OrdersController(LogiTrackContext context, IMemoryCache cache)
        {
            dbContext = context;
            _cache = cache;
        }

        // Return a list of all orders
        [HttpGet]
        public async Task<IActionResult> GetOrdersAsync()
        {
            if (_cache.TryGetValue(CacheKey, out var cachedOrders))
            {
                Console.WriteLine("Returning cached orders.");
                return Ok(cachedOrders);
            }

            var order = await dbContext.Orders
                .AsNoTracking()
                .Include(o => o.ItemList).ToListAsync();

            if (order == null || !order.Any())
            {
                return NotFound("No orders found.");
            }

            _cache.Set(CacheKey, order, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
            });

            return Ok(order);
        }

        // Return an order with its items
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderByIdAsync(int id)
        {
            var order = await dbContext.Orders
                .AsNoTracking()
                .Include(o => o.ItemList)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null) return NotFound($"Order with ID {id} not found.");

            return Ok(order);
        }

        // Create a new order
        [HttpPost]
        public async Task<IActionResult> CreateOrderAsync([FromBody] Order order)
        {
            if (order == null) return BadRequest("Order cannot be null.");

            _cache.Remove(CacheKey);

            await dbContext.Orders.AddAsync(order);
            await dbContext.SaveChangesAsync();
            var updatedOrders = await dbContext.Orders
                .Include(o => o.ItemList)
                .ToListAsync(); // Ensure the cache is updated

            _cache.Set(CacheKey, updatedOrders, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
            });

            return CreatedAtAction(nameof(GetOrderByIdAsync), new { id = order.OrderId }, order);
        }

        // Delete an order by ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderByIdAsync(int id)
        {
            var order = await dbContext.Orders.FindAsync(id);

            if (order == null) return NotFound($"Order with ID {id} not found.");

            dbContext.Orders.Remove(order);
            await dbContext.SaveChangesAsync();

            _cache.Remove("Orders"); // Clear cache after deletion

            return NoContent();
        }
    }
}