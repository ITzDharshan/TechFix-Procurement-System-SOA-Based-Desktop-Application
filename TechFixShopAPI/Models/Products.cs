namespace TechFixShopAPI.Models
{
    public class Products
    {
        public int ProductId { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public string SupplierUsername { get; set; }
    }
}
