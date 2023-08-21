using ASP.NET_CA_SEVEN_SHOP.Models;

namespace ASP.NET_CA_SEVEN_SHOP.ViewModels
{
    public class CartItemsViewModel
    {
        public int ProductId {  get; set; }

        public Product Product { get; set; }

        public int Quantity { get; set; }
    }
}
