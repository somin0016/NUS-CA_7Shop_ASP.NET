using Microsoft.EntityFrameworkCore;

namespace ASP.NET_CA_SEVEN_SHOP.Models
{
    [PrimaryKey("ShoppingCartId", "ProductId")]
    public class CartItem
    {
        public int ShoppingCartId { get; set; }
        public virtual ShoppingCart ShoppingCart { get; set; }

        public int ProductId { get; set; }

        public virtual Product Product { get; set; }

        public int Quantity {  get; set; }
    }
}
