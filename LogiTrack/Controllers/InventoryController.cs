using System.Diagnostics;
using LogiTrack.Controllers.Interfaces;
using LogiTrack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace LogiTrack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase, IInventoryService
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
        public async Task<IActionResult> GetAllInventoryItemsAsync()
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

            var inventoryItems = await dbContext.InventoryItems
                .AsNoTracking()
                .ToListAsync();

            stopwatch.Stop();
            Console.WriteLine($"GetAllInventoryItems executed in {stopwatch.ElapsedMilliseconds} ms");

            if (inventoryItems == null)
            {
                return NotFound("No inventory items found.");
            }
            else
            {
                _cache.Set(CacheKey, inventoryItems, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                });

                return Ok(inventoryItems);
            }
        }

        // Add a new item to the inventory
        [HttpPost]
        public async Task<IActionResult> AddInventoryItemAsync([FromBody] InventoryItem newItem)
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
                _cache.Remove(CacheKey); // Clear cache before adding a new item

                await dbContext.InventoryItems.AddAsync(newItem);
                await dbContext.SaveChangesAsync();

                var updatedInventory = await dbContext.InventoryItems.ToListAsync(); // Ensure the cache is updated
                _cache.Set(CacheKey, updatedInventory, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                });

                return Ok(newItem);
            }
        }

        // Remove an item by ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveInventoryItemByIdAsync(int id)
        {
            var item = await dbContext.InventoryItems.FindAsync(id);
            if (item == null) return NotFound($"Item with ID {id} not found.");

            dbContext.InventoryItems.Remove(item);
            await dbContext.SaveChangesAsync();

            _cache.Remove(CacheKey); // Clear cache after deletion

            return Ok($"Item with ID {id} removed successfully.");
        }
    }
}