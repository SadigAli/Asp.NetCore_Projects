
namespace Leka.Models
{
    public class ProductBasket
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } 
        public string CategoryName { get; set; }
        public double SalePrice { get; set; }
        public int Count { get; set; }
        public string Image { get; set; }
        public AppUser AppUser { get; set; }
        public string AppUserId { get; set; }
    }
}