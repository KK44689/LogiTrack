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
    public class OrdersController : ControllerBase
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
        public async Task<IActionResult> GetOrders()
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
        public async Task<IActionResult> GetOrderById(int id)
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
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            if (order == null) return BadRequest("Order cannot be null.");

            await dbContext.Orders.AddAsync(order);
            await dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrderById), new { id = order.OrderId }, order);
        }

        // Delete an order by ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderById(int id)
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