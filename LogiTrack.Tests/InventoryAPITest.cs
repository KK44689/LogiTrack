using LogiTrack.Models;
using Moq;
using Microsoft.AspNetCore.Mvc;
using LogiTrack.Controllers.Interfaces;

namespace LogiTrack.Tests;

public class InventoryAPITest
{
    [Fact]
    public async Task TestGetAllInventoryItems()
    {
        var mockResult = new List<InventoryItem>
        {
            new InventoryItem { ItemId = 1, Name = "Item1", Quantity = 10, Location = "Warehouse A" },
            new InventoryItem { ItemId = 2, Name = "Item2", Quantity = 20, Location = "Warehouse B" }
        };

        var controller = new Mock<IInventoryService>();
        controller.Setup(c => c.GetAllInventoryItemsAsync())
            .Returns(Task.FromResult<IActionResult>(new OkObjectResult(mockResult)));

        var result = await controller.Object.GetAllInventoryItemsAsync();
        var okResult = Assert.IsType<OkObjectResult>(result);
        var item = Assert.IsType<List<InventoryItem>>(okResult.Value);
        Assert.Equal(mockResult, item);
    }

    [Fact]
    public async Task TestAddInventoryItem_ReturnsCreated()
    {
        var newItem = new InventoryItem { ItemId = 3, Name = "Item3", Quantity = 5, Location = "Warehouse C" };
        var controller = new Mock<IInventoryService>();
        controller.Setup(c => c.AddInventoryItemAsync(newItem))
            .Returns(Task.FromResult<IActionResult>(new CreatedAtActionResult("GetInventoryItemById", "Inventory", new { id = newItem.ItemId }, newItem)));

        var result = await controller.Object.AddInventoryItemAsync(newItem);
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var item = Assert.IsType<InventoryItem>(createdResult.Value);
        Assert.Equal("Item3", item.Name);
    }

    [Fact]
    public async Task TestDeleteInventoryItem_ReturnsNoContent()
    {
        var controller = new Mock<IInventoryService>();
        controller.Setup(c => c.RemoveInventoryItemByIdAsync(1))
            .Returns(Task.FromResult<IActionResult>(new NoContentResult()));

        var result = await controller.Object.RemoveInventoryItemByIdAsync(1);
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task TestDeleteInventoryItem_ReturnsNotFound()
    {
        var controller = new Mock<IInventoryService>();
        controller.Setup(c => c.RemoveInventoryItemByIdAsync(99))
            .Returns(Task.FromResult<IActionResult>(new NotFoundResult()));

        var result = await controller.Object.RemoveInventoryItemByIdAsync(99);
        Assert.IsType<NotFoundResult>(result);
    }
}
