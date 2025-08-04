using LogiTrack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogiTrack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly LogiTrackContext dbContext;

        public InventoryController(LogiTrackContext context)
        {
            dbContext = context;
        }

        // Return a list of all inventory items
        [HttpGet]
        public async Task<IActionResult> GetAllInventoryItems()
        {
            var inventoryItems = await dbContext.InventoryItems.ToListAsync();

            if (inventoryItems == null)
            {
                return NotFound("No inventory items found.");
            }
            else
            {
                return Ok(inventoryItems);
            }
        }

        // Add a new item to the inventory
        [HttpPost]
        public async Task<IActionResult> AddInventoryItem([FromBody] InventoryItem newItem)
        {
            if (newItem == null)
            {
                return BadRequest("Invalid inventory item.");
            }

            var isItemExists = await dbContext.InventoryItems.AnyAsync(i => i == newItem);

            if (isItemExists)
            {
                return Conflict("The item already exists.");
            }
            else
            {
                await dbContext.InventoryItems.AddAsync(newItem);
                await dbContext.SaveChangesAsync();
                Console.WriteLine($"Item added: {newItem.ItemId}:{newItem.Name}, Quantity: {newItem.Quantity}, Location: {newItem.Location}");
                return Ok(newItem);
            }
        }

        // Remove an item by ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveInventoryItemById(int id)
        {
            var item = await dbContext.InventoryItems.FindAsync(id);
            if (item == null) return NotFound($"Item with ID {id} not found.");

            dbContext.InventoryItems.Remove(item);
            await dbContext.SaveChangesAsync();
            return Ok($"Item with ID {id} removed successfully.");
        }
    }
}