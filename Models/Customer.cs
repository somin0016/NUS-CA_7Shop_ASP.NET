using System.ComponentModel.DataAnnotations;

namespace ASP.NET_CA_SEVEN_SHOP.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public virtual ICollection<Order> Order { get; set; }
    }
}