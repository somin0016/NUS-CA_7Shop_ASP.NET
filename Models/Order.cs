using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASP.NET_CA_SEVEN_SHOP.Models
{
    public class Order
    {
        // Constructor
        public Order()
        {
            OrderDateTime = DateTime.Now;    
        }

        [Key]
        public int OrderId { get; set; }

        public DateTime OrderDateTime { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }

        public virtual ICollection<OrderDetail> OrderDetail { get; set; }
    }
}
