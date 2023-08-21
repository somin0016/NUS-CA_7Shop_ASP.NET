using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ASP.NET_CA_SEVEN_SHOP.Models;
using ASP.NET_CA_SEVEN_SHOP.Data;

namespace ASP.NET_CA_SEVEN_SHOP.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _db;

        public ProductController(AppDbContext db)
        {
            _db = db;
        }

        public IActionResult Index(string search)
        {
            /* Retrieving Data from Database */
            List<Product> products = _db.Products.ToList();

            /* Filter the products list based on the searchStr query string parameter */
            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(product =>
                    product.ProductName.ToLower().Contains(search.ToLower())
                    || product.ProductKeywords!.ToLower().Contains(search.ToLower())
                    || product.ProductDescription.ToLower().Contains(search.ToLower())
                ).ToList();
            }

            /* Sending Data to View */
            ViewBag.search = search;

            return View("Gallery", products);
        }
    }
}
