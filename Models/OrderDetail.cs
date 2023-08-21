using Microsoft.EntityFrameworkCore;

namespace ASP.NET_CA_SEVEN_SHOP.Models
{
    [PrimaryKey("OrderId", "ProductId")]
    public class OrderDetail
    {
        // Constructor
        public OrderDetail()
        {
            CreatedAt = DateTime.Now;
        }

        public int OrderId { get; set; }
        public virtual Order Order { get; set; }

        public int ProductId { get; set; }

        public virtual Product Product { get; set; }

        public int Quantity { get; set; }

        public int ReviewRating { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual ICollection<ActivationCode> ActivationCodes { get; set; }
    }
}
