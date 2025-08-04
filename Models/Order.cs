using System.ComponentModel.DataAnnotations;

namespace LogiTrack.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; } // Primary key

        public required string CustomerName { get; set; }
        public required DateTime DatePlaced { get; set; }
        public List<InventoryItem> ItemList { get; set; } = new List<InventoryItem>();

        public void AddItem(InventoryItem item)
        {
            if (!ItemList.Exists(i => i.ItemId == item.ItemId))
            {
                ItemList.Add(item);
            }
            else
            {
                Console.WriteLine($"Item with ID {item.ItemId} already exists in the order.");
            }
        }

        public void RemoveItem(int itemId)
        {
            if (ItemList.Exists(item => item.ItemId == itemId))
            {
                ItemList.Remove(ItemList.Find(item => item.ItemId == itemId));
            }
            else
            {
                Console.WriteLine($"Item with ID {itemId} not found in the order.");
            }
        }

        public string GetOrderSummary()
        {
            var summary = $"Order {OrderId} for {CustomerName} | Items: {ItemList.Count} | Placed: {DatePlaced}";
            return summary;
        }
    }
}