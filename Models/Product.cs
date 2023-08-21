using System.ComponentModel.DataAnnotations;

namespace ASP.NET_CA_SEVEN_SHOP.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        public string ProductName { get; set; }

        public string ProductDescription { get; set; }

        [Required]
        public double ProductPrice { get; set; }

        public string? ProductCategory { get; set; }

        public string? ProductKeywords { get; set; }

        [Required]
        public double ProductRating { get; set; }

        [Required]
        public int ProductReviewers { get; set; }

        public string ProductImageUrl { get; set; }

        public virtual ICollection<CartItem> CartItem { get; set; }
    }
}
