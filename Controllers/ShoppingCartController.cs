using ASP.NET_CA_SEVEN_SHOP.Data;
using ASP.NET_CA_SEVEN_SHOP.Models;
using ASP.NET_CA_SEVEN_SHOP.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ASP.NET_CA_SEVEN_SHOP.Controllers
{
    public class ShoppingCartController : Controller
    {
        // Attributes
        private readonly AppDbContext _db;
        private List<CartItemsViewModel> vmCartItems;

        // Constructor
        public ShoppingCartController(AppDbContext db)
        {
            _db = db;
            vmCartItems = new List<CartItemsViewModel>();
        }

        public IActionResult Index()
        {
            ISession session = HttpContext.Session;
            string sessUsername = session.GetString("Username");
            string sessCartItems = session.GetString("CartItems");

            /* Checking Customer Login State */
            if (sessUsername == null) ViewData["Username"] = "Guest User";
            else ViewData["Username"] = sessUsername;

            /* For Guest Users, retrieve the Cart Items in the Session */
            if (sessUsername == null)
            {
                if (sessCartItems != null)
                {
                    Dictionary<int, int> cartItems = JsonSerializer.Deserialize<Dictionary<int, int>>(sessCartItems);

                    foreach (KeyValuePair<int, int> item in cartItems)
                    {
                        vmCartItems.Add(new CartItemsViewModel
                        {
                            Product = _db.Products.FirstOrDefault(x => x.ProductId == item.Key),
                            Quantity = item.Value
                        });
                    }
                }
            }
            /* For Logged-In Users, retrieve the Cart Items from the Database */
            else
            {
                ShoppingCart customerCart = _db.ShoppingCarts.FirstOrDefault(cart => cart.Customer.Username == sessUsername);
                if (customerCart == null) return View("ViewCart", vmCartItems);

                foreach (CartItem cartItem in customerCart.CartItem)
                {
                    vmCartItems.Add(new CartItemsViewModel
                    {
                        Product = cartItem.Product,
                        Quantity = cartItem.Quantity
                    });
                }
            }

            return View("ViewCart", vmCartItems);
        }

        [HttpPost]
        public IActionResult AddItem([FromBody] CartItemsViewModel vm)
        {
            ISession session = HttpContext.Session;
            string sessUsername = session.GetString("Username");
            string sessCartItems = session.GetString("CartItems");

            /* Adding Products by Guest User */
            if (sessUsername == null)
            {
                Dictionary<int, int> cartItems;

                /* 
                 * If 'CartItems' is not in the session,
                 * initialize an empty Dictionary object for 'CartItems'.
                 */
                if (sessCartItems == null)
                    cartItems = new Dictionary<int, int>();
                /*
                 * If 'CartItems' is already in the session,
                 * convert the object string to the Dictionary object.
                 */
                else
                    cartItems = JsonSerializer.Deserialize<Dictionary<int, int>>(sessCartItems);

                if (!cartItems.ContainsKey(vm.ProductId))
                    cartItems.Add(vm.ProductId, 1);
                else
                    cartItems[vm.ProductId]++;

                /* Saving the 'CartItems' in the session. */
                session.SetString("CartItems", JsonSerializer.Serialize(cartItems));
                session.SetInt32("CartItemsCount", CartItemsCount(cartItems));

                int totalQty = 0;

                foreach (var itemQuantity in cartItems)
                {
                    totalQty += itemQuantity.Value;
                }

                ViewBag.TotalQty = totalQty;

                return Ok(new
                {
                    success = true,
                    productId = vm.ProductId,
                    totalQty = totalQty
                });
            }
            /* Adding Products by Logged In User */
            else
            {
                ShoppingCart customerCart = _db.ShoppingCarts.FirstOrDefault(cart => cart.Customer.Username == sessUsername);

                if (customerCart == null)
                {
                    _db.ShoppingCarts.Add(new ShoppingCart { Customer = _db.Customers.FirstOrDefault(customer => customer.Username == sessUsername), CartItem = new List<CartItem>() });
                    _db.SaveChanges();
                    customerCart = _db.ShoppingCarts.FirstOrDefault(cart => cart.Customer.Username == sessUsername);
                }

                Product addedProduct = _db.Products.FirstOrDefault(x => x.ProductId == vm.ProductId);

                /* Check whether there are already existing products in the Customer's Shopping Cart */
                if (customerCart.CartItem.Count == 0)
                    customerCart.CartItem.Add(new CartItem { Product = addedProduct, Quantity = 1 });
                else
                {
                    /* Check whether the Added Item is already existing in the Customer's Shopping Cart */
                    CartItem existingItem = customerCart.CartItem.FirstOrDefault(x => x.Product.ProductId == vm.ProductId);
                    /* If NOT EXISTS, ADD the CartItem to the Shopping Cart */
                    if (existingItem == null)
                        customerCart.CartItem.Add(new CartItem { Product = addedProduct, Quantity = 1 });
                    /* If EXISTS, UPDATE the CartItem in the Shopping Cart */
                    else
                        existingItem.Quantity++;
                }
                _db.SaveChanges();

                var totalQty = 0;
                foreach (var cartItem in customerCart.CartItem)
                {
                    totalQty += cartItem.Quantity;
                }
                session.SetInt32("CartItemsCount", totalQty);

                return Ok(new
                {
                    success = true,
                    productId = vm.ProductId,
                    totalQty = totalQty
                });
            }
        }

        /* If the Customer is NOT logged in, store the updated quantity in session
         * else updates quantity in database */
        [HttpPost]
        public IActionResult CartUpdate([FromBody] CartItemsViewModel vm)
        {
            ISession session = HttpContext.Session;
            string sessUsername = session.GetString("Username");
            string sessCartItems = session.GetString("CartItems");

            if (sessUsername == null)
            {
                Dictionary<int, int> cartItems;
                cartItems = JsonSerializer.Deserialize<Dictionary<int, int>>(sessCartItems);

                if (cartItems.ContainsKey(vm.ProductId))
                {
                    cartItems[vm.ProductId] = vm.Quantity;
                }

                session.SetString("CartItems", JsonSerializer.Serialize(cartItems));
                session.SetInt32("CartItemsCount", CartItemsCount(cartItems));

                return Ok(new
                {
                    success = true,
                    productId = vm.ProductId
                });
            }
            else
            {
                ShoppingCart customerCart = _db.ShoppingCarts.FirstOrDefault(cart => cart.Customer.Username == sessUsername);
                CartItem updatedCartItem = customerCart.CartItem.FirstOrDefault(cartItem => cartItem.Product.ProductId == vm.ProductId);

                if (updatedCartItem != null)
                {
                    updatedCartItem.Quantity = vm.Quantity;
                    _db.SaveChanges();

                    var totalQty = 0;
                    foreach (var cartItem in customerCart.CartItem)
                    {
                        totalQty += cartItem.Quantity;
                    }
                    session.SetInt32("CartItemsCount", totalQty);


                    return Ok(new
                    {
                        success = true,
                        productId = vm.ProductId,
                        quantity = vm.Quantity
                    });
                }

                return NotFound(new
                {
                    success = false
                });
            }
        }

        public IActionResult RemoveItem(int id)
        {
            ISession session = HttpContext.Session;
            string sessUsername = session.GetString("Username");
            string sessCartItems = session.GetString("CartItems");

            /* Removing Products by Guest User */
            if (sessUsername == null)
            {
                if (sessCartItems != null)
                {
                    Dictionary<int, int> cartItems;
                    cartItems = JsonSerializer.Deserialize<Dictionary<int, int>>(sessCartItems);
                    cartItems.Remove(id);
                    session.SetString("CartItems", JsonSerializer.Serialize(cartItems));
                    session.SetInt32("CartItemsCount", CartItemsCount(cartItems));
                }
            }
            /* Removing Products by Logged In User */
            else
            {
                ShoppingCart customerCart = _db.ShoppingCarts.FirstOrDefault(cart => cart.Customer.Username == sessUsername);

                CartItem removedProduct = customerCart.CartItem.FirstOrDefault(cartItem => cartItem.Product.ProductId == id);
                customerCart.CartItem.Remove(removedProduct);
                _db.SaveChanges();
                var totalQty = 0;
                foreach (var cartItem in customerCart.CartItem)
                {
                    totalQty += cartItem.Quantity;
                }
                session.SetInt32("CartItemsCount", totalQty);
            }

            return RedirectToAction("Index");
        }

        private int CartItemsCount(Dictionary<int, int> cartItems)
        {
            int sum = 0;
            foreach (var cartItemValue in cartItems.Values)
            {
                sum += cartItemValue;
            }

            return sum;
        }

        public IActionResult Test()
        {
            List<Product> products = new List<Product>();
            for (int i = 1; i < 8; i++)
            {
                products.Add(_db.Products.FirstOrDefault(x => x.ProductId == i));
            }

            Customer customer = _db.Customers.FirstOrDefault(x => x.CustomerId == 1);

            ShoppingCart toRemove = _db.ShoppingCarts.FirstOrDefault(x => x.ShoppingCartId == 1);
            _db.ShoppingCarts.Remove(toRemove);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult SeedCarts()
        {
            List<Customer> customers = new List<Customer>();
            for (int i = 1; i < 8; i++)
            {
                customers.Add(_db.Customers.FirstOrDefault(x => x.CustomerId == i));
            }

            List<CartItem> products = new List<CartItem>();

            foreach (Customer customer in customers)
            {
                _db.ShoppingCarts.Add(new ShoppingCart { Customer = customer, CartItem = products });
            }

            _db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
