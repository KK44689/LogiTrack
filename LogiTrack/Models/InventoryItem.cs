using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogiTrack.Models
{
    public class InventoryItem
    {
        [Key]
        public int ItemId { get; set; } // Primary key

        public required string Name { get; set; }
        public required int Quantity { get; set; }
        public required string Location { get; set; }

        [ForeignKey("Order")]
        public int OrderId { get; set; } // Foreign key
        public Order? Order { get; set; } // Navigation property

        public void DisplayInfo()
        {
            var info = $"Item: {Name} | Quantity: {Quantity} | Location: {Location}";
            Console.WriteLine(info);
        }
    }
}