using System;

namespace TechFixShopAPI.Models
{
    public class Quotations
    {
        public int Id { get; set; }
        public string ItemName { get; set; }
        public decimal Price { get; set; }
        public string SupplierUsername { get; set; }
        public string Response { get; set; }
        public string Status { get; set; }
        public DateTime RequestDate { get; set; }
    }
}