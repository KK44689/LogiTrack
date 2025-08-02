class TestController
{
    public void TestDisplayInfo()
    {
        var inventoryItem = new InventoryItem
        {
            ItemId = 1,
            Name = "Sample Item",
            Quantity = 10,
            Location = "Warehouse A"
        };

        inventoryItem.DisplayInfo();
    }

    public void TestGetOrderSummary()
    {
        var order = new Order
        {
            OrderId = 123,
            CustomerName = "John Doe",
            DatePlaced = DateTime.Now
        };

        order.AddItem(new InventoryItem { ItemId = 1, Name = "Item A", Quantity = 5, Location = "Shelf 1" });
        order.AddItem(new InventoryItem { ItemId = 2, Name = "Item B", Quantity = 3, Location = "Shelf 2" });

        Console.WriteLine(order.GetOrderSummary());
    }

    public void TestRemoveItem()
    {
        var order = new Order
        {
            OrderId = 123,
            CustomerName = "John Doe",
            DatePlaced = DateTime.Now
        };

        order.AddItem(new InventoryItem { ItemId = 1, Name = "Item A", Quantity = 5, Location = "Shelf 1" });
        order.RemoveItem(1); // Should remove Item A

        Console.WriteLine(order.GetOrderSummary());
    }

    public void TestDatabaseContext()
    {
        using (var context = new LogiTrackContext())
        {
            // First, create and save an order
            var order = new Order { CustomerName = "Test", DatePlaced = DateTime.Now };
            context.Orders.Add(order);
            context.SaveChanges();

            // Then, add an inventory item linked to that order
            var name = "Pallet Jack";
            var quantity = 12;
            var location = "Warehouse A";
            
            bool isItemExists = context.InventoryItems.Any(item =>
                item.Name == name &&
                item.Quantity == quantity &&
                item.Location == location
            );

            if (!isItemExists)
            {
                context.InventoryItems.Add(new InventoryItem
                {
                    Name = "Pallet Jack",
                    Quantity = 12,
                    Location = "Warehouse A",
                    OrderId = order.OrderId
                });

                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("Item already exists in the inventory.");
            }

            // Retrieve and print inventory to confirm
            var items = context.InventoryItems.ToList();
            foreach (var item in items)
            {
                item.DisplayInfo(); // Should print: Item: Pallet Jack | Quantity: 12 | Location: Warehouse A
            }
        }
    }
}