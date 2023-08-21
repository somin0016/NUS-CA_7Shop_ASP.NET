using System.ComponentModel.DataAnnotations;

namespace ASP.NET_CA_SEVEN_SHOP.Models
{
    public class ActivationCode
    {
        // Constructor
        public ActivationCode()
        {
            CreatedAt = DateTime.Now;
        }

        [Key]
        public string ActivationCodeId { get; set; }

        public virtual OrderDetail OrderDetail { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
