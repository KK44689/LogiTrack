using LogiTrack.Models;
using Microsoft.AspNetCore.Mvc;

namespace LogiTrack.Controllers.Interfaces
{
    public interface IInventoryService
    {
        Task<IActionResult> GetAllInventoryItemsAsync();
        Task<IActionResult> AddInventoryItemAsync([FromBody] InventoryItem newItem);
        Task<IActionResult> RemoveInventoryItemByIdAsync(int id);
    }
}