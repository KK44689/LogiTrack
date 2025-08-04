using LogiTrack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogiTrack.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly LogiTrackContext dbContext;

        public OrdersController(LogiTrackContext context)
        {
            dbContext = context;
        }

        // Return a list of all orders
        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var order = await dbContext.Orders
                .Include(o => o.ItemList).ToListAsync();

            if (order == null || !order.Any())
            {
                return NotFound("No orders found.");
            }

            return Ok(order);
        }

        // Return an order with its items
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await dbContext.Orders
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

            return NoContent();
        }
    }
}