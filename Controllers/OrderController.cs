using ASP.NET_CA_SEVEN_SHOP.Data;
using ASP.NET_CA_SEVEN_SHOP.Models;
using ASP.NET_CA_SEVEN_SHOP.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_CA_SEVEN_SHOP.Controllers
{
    public class OrderController : Controller
    {
        // Attributes
        private readonly AppDbContext _db;

        // Constructor
        public OrderController(AppDbContext db)
        {
            _db = db;    
        }

        public IActionResult Index()
        {
            ISession session = HttpContext.Session;
            string? sessUsername = session.GetString("Username");

            /* Check Customer's Login Status */
            if (sessUsername == null) return RedirectToAction("Login", "Customer");

            /* Retrieve the Orders made by the Customer */
            List<Order> customerOrders = _db.Orders.Where(order => order.Customer.Username == sessUsername).ToList();

            List<OrderDetail> orderItems = new List<OrderDetail>();
            
            foreach(Order order in customerOrders)
            {
                orderItems.AddRange(order.OrderDetail.ToList());
            }

            return View("PurchaseHistory", orderItems);
        }

        public IActionResult Checkout()
        {
            ISession session = HttpContext.Session;
            string? sessUsername = session.GetString("Username");

            /* If Customer is NOT logged in, redirect to the Login Page */
            if (sessUsername == null) return RedirectToAction("Login", "Customer");

            /* If Customer is ALREADY logged in */
            ShoppingCart customerCart = _db.ShoppingCarts.FirstOrDefault(cart => cart.Customer.Username == sessUsername);
            List<CartItem> cartItems = customerCart.CartItem.ToList();

            /* If the Customer's Shopping Cart is EMPTY */
            if (cartItems.Count == 0) return RedirectToAction("Index", "ShoppingCart");

            /* Creating a new Order object */
            Order newOrder = new Order { Customer = customerCart.Customer };

            /* Retrieving the Cart Items from the Customer's Shopping Cart */
            List<OrderDetail> orderItems = new List<OrderDetail>();
            foreach (CartItem cartItem in cartItems)
            {
                orderItems.Add(new OrderDetail { Order = newOrder, Product = cartItem.Product, Quantity = cartItem.Quantity });
            }

            /* Inserting the Order Items (Order Details) into the created Order object */
            newOrder.OrderDetail = orderItems;

            CreateActivationCode(orderItems);

            /* Removing all Cart Items from the Shopping Cart */
            customerCart.CartItem.Clear();

            _db.Orders.Add(newOrder);
            _db.SaveChanges();

            /* Resets the cart qty after checkout */
            session.SetInt32("CartItemsCount", 0);

            return RedirectToAction("Index", "Order");
        }

        public IActionResult Review([FromBody] ReviewRatingViewModel vm)
        {
            Order order = _db.Orders.FirstOrDefault(order => order.OrderId == vm.OrderId);
            OrderDetail reviewItem = order.OrderDetail.FirstOrDefault(item => item.ProductId == vm.ProductId);

            /* Retrieving the existing Average Rating & Reviewers from the Product */
            int productReviewers = reviewItem.Product.ProductReviewers;
            double productRating = reviewItem.Product.ProductRating;


            /* Calculating the new Average Rating & Reviewers for the Product */
            int newProductReviewers = 0;
            double newProductRating = 0.0;

            if (reviewItem.ReviewRating == 0)
            {
                /* Customer has not made the Review yet */
                newProductReviewers = productReviewers + 1;
                newProductRating = ((productRating * productReviewers) + vm.ReviewRating) / newProductReviewers;
            }
            else
            {
                /* Customer has already made the Review and attempt to change the Review */
                newProductReviewers = productReviewers;
                newProductRating = ((productRating * productReviewers) - reviewItem.ReviewRating + vm.ReviewRating) / productReviewers;
            }

            /* Update the new Average Product Rating & Number of Reviewers in the Product table */
            Product newProduct = reviewItem.Product;
            newProduct.ProductRating = newProductRating;
            newProduct.ProductReviewers = newProductReviewers;

            /* Update the Customer's Review Rating in the Order Detail */
            reviewItem.ReviewRating = vm.ReviewRating;

            _db.OrderDetails.Update(reviewItem);
            _db.Products.Update(newProduct);
            _db.SaveChanges();

            return Ok(new
            {
                success = true,
                orderId = vm.OrderId,
                productId = vm.ProductId,
                rating = vm.ReviewRating
            });
        }

        private void CreateActivationCode(List<OrderDetail> orderItems)
        {
            List<ActivationCode> activationCodes = new List<ActivationCode>();
            foreach(OrderDetail orderItem in orderItems)
            {
                for (int qty = 0; qty < orderItem.Quantity; qty++)
                {
                    String ac = GenerateActivationCode();
                    _db.ActivationCodes.Add(new ActivationCode { ActivationCodeId = ac, OrderDetail = orderItem });
                }
            }
        }

        private string GenerateActivationCode()
        {
            Random random = new Random();
            string[] generate = {
                    Convert.ToChar(random.Next(97, 123)).ToString() + random.Next(1, 10000).ToString("D4"),
                    random.Next(1, 1000).ToString("D3"),
                    Convert.ToChar(random.Next(97, 123)).ToString() + Convert.ToChar(random.Next(97, 123)).ToString() + Convert.ToChar(random.Next(97, 123)).ToString() + Convert.ToChar(random.Next(97, 123)).ToString(),
                    random.Next(1, 1000).ToString("D3")
                };
            string ActivationCode = string.Join("-", generate).ToUpper();
            return ActivationCode;
        }
    }
}
