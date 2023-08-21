/* Implemented by Junwei */
using ASP.NET_CA_SEVEN_SHOP.Data;
using ASP.NET_CA_SEVEN_SHOP.Models;
using ASP.NET_CA_SEVEN_SHOP.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ASP.NET_CA_SEVEN_SHOP.Controllers
{
    public class CustomerController : Controller
    {
        // Attributes
        private readonly AppDbContext _db;

        // Constructor
        public CustomerController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("CustomerId") != null)
            {
                return RedirectToAction("Login", "Customer");
            }
            
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            ISession session = HttpContext.Session;
            string? sessCartItems = session.GetString("CartItems");

            /* If the submitted Customer's Username or Password is Empty */
            if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
            {
                ViewBag.ErrorMsg = "Username or Password must not be empty";
                return View();
            }

            /* If the Customer data does not exist in the database */
            Customer? customer = _db.Customers.FirstOrDefault(customer => customer.Username == username);
            if (customer == null)
            {
                ViewBag.ErrorMsg = "Username cannot be found";
                return View();
            }

            if (customer != null && customer.Password.Equals(password, StringComparison.Ordinal))
            {
                session.SetInt32("CustomerId", customer.CustomerId);
                session.SetString("Username", customer.Username);

                MergeShoppingCart(customer, sessCartItems);

                return RedirectToAction("Index", "Product");
            }
            else
            {
                ViewBag.ErrorMsg = "Invalid Username or Password";
                return View();
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Product");
        }

        private void MergeShoppingCart(Customer customer, string sessCartItems)
        {
            /* If the Customer's Shopping Cart is not created yet, create a new Shopping Cart */
            ShoppingCart customerCart = _db.ShoppingCarts.FirstOrDefault(cart => cart.Customer.Username == customer.Username);
            if (customerCart == null)
            {
                _db.ShoppingCarts.Add(new ShoppingCart { Customer = customer, CartItem = new List<CartItem>() });
                _db.SaveChanges();
                customerCart = _db.ShoppingCarts.FirstOrDefault(cart => cart.Customer.Username == customer.Username);
            }

            if (sessCartItems != null)
            {
                Dictionary<int, int> cartItems = JsonSerializer.Deserialize<Dictionary<int, int>>(sessCartItems);

                foreach (KeyValuePair<int, int> cartItem in cartItems)
                {
                    /* Check whether the Added Item is already existing in the Customer's Shopping Cart */
                    CartItem existingItem = customerCart.CartItem.FirstOrDefault(item => item.Product.ProductId == cartItem.Key);
                    /* If NOT EXISTS, ADD the CartItem to the Shopping Cart */
                    if (existingItem == null)
                    {
                        Product addedProduct = _db.Products.FirstOrDefault(product => product.ProductId == cartItem.Key);
                        customerCart.CartItem.Add(new CartItem { Product = addedProduct, Quantity = cartItem.Value });
                        _db.ShoppingCarts.Update(customerCart);
                    }
                    /* If EXISTS, UPDATE the CartItem in the Shopping Cart */
                    else
                    {
                        existingItem.Quantity = cartItem.Value;
                        _db.ShoppingCarts.Update(customerCart);
                    }
                }
                _db.SaveChanges();
            }

            /** Updates the total cart qty after merging **/
            var totalQty = 0;
            foreach (var cartItem in customerCart.CartItem)
            {
                totalQty += cartItem.Quantity;
            }
            HttpContext.Session.SetInt32("CartItemsCount", totalQty);
        }
    }
}
