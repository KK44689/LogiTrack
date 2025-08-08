using LogiTrack.Controllers.Interfaces;
using LogiTrack.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

public class OrderAPITest
{
    [Fact]
    public async Task TestGetAllOrders()
    {
        var mockResult = new List<Order>
        {
            new Order { OrderId = 1, CustomerName = "Customer1", DatePlaced = DateTime.UtcNow },
            new Order { OrderId = 2, CustomerName = "Customer2", DatePlaced = DateTime.UtcNow }
        };

        var controller = new Mock<IOrderService>();
        controller.Setup(c => c.GetOrdersAsync())
            .Returns(Task.FromResult<IActionResult>(new OkObjectResult(mockResult)));

        var result = await controller.Object.GetOrdersAsync();
        var okResult = Assert.IsType<OkObjectResult>(result);
        var orders = Assert.IsType<List<Order>>(okResult.Value);
    }

    [Fact]
    public async Task TestGetOrderById()
    {
        var mockOrder = new Order { OrderId = 1, CustomerName = "Customer1", DatePlaced = DateTime.UtcNow };
        var controller = new Mock<IOrderService>();
        controller.Setup(c => c.GetOrderByIdAsync(1))
            .Returns(Task.FromResult<IActionResult>(new OkObjectResult(mockOrder)));

        var result = await controller.Object.GetOrderByIdAsync(1);
        var okResult = Assert.IsType<OkObjectResult>(result);
        var order = Assert.IsType<Order>(okResult.Value);
        Assert.Equal("Customer1", order.CustomerName);
    }

    [Fact]
    public async Task TestCreateOrder_ReturnsCreated()
    {
        var newOrder = new Order { OrderId = 3, CustomerName = "Customer3", DatePlaced = DateTime.UtcNow };
        var controller = new Mock<IOrderService>();
        controller.Setup(c => c.CreateOrderAsync(newOrder))
            .Returns(Task.FromResult<IActionResult>(new CreatedAtActionResult("GetOrderById", "Orders", new { id = newOrder.OrderId }, newOrder)));

        var result = await controller.Object.CreateOrderAsync(newOrder);
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var order = Assert.IsType<Order>(createdResult.Value);
        Assert.Equal("Customer3", order.CustomerName);
    }

    [Fact]
    public async Task TestDeleteOrder_ReturnsNoContent()
    {
        var controller = new Mock<IOrderService>();
        controller.Setup(c => c.DeleteOrderByIdAsync(1))
            .Returns(Task.FromResult<IActionResult>(new NoContentResult()));

        var result = await controller.Object.DeleteOrderByIdAsync(1);
        Assert.IsType<NoContentResult>(result);
    }
}