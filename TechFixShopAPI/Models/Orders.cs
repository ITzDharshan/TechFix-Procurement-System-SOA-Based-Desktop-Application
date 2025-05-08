using System;

namespace TechFixShopAPI.Models
{
    public class Orders
    {
        public int OrderId { get; set; }
        public string SupplierUsername { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal? Discount { get; set; }
        public string Status { get; set; }
        public DateTime? OrderDate { get; set; }
    }

}
