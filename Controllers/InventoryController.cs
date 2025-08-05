using System.Diagnostics;
using LogiTrack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace LogiTrack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly LogiTrackContext dbContext;
        private readonly IMemoryCache _cache;

        private const string CacheKey = "InventoryItems";

        public InventoryController(LogiTrackContext context, IMemoryCache cache)
        {
            dbContext = context;
            _cache = cache;
        }

        // Return a list of all inventory items
        [HttpGet]
        public async Task<IActionResult> GetAllInventoryItems()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            if (_cache.TryGetValue(CacheKey, out var cachedItems))
            {
                stopwatch.Stop();
                Console.WriteLine($"Cached GetAllInventoryItems executed in {stopwatch.ElapsedMilliseconds} ms");
                Console.WriteLine("Returning cached inventory items.");

                return Ok(cachedItems);
            }

            var inventoryItems = await dbContext.InventoryItems.ToListAsync();

            stopwatch.Stop();
            Console.WriteLine($"GetAllInventoryItems executed in {stopwatch.ElapsedMilliseconds} ms");

            if (!inventoryItems.Any())
            {
                return NotFound("No inventory items found.");
            }

            _cache.Set(CacheKey, inventoryItems, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
            });

            return Ok(inventoryItems);
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