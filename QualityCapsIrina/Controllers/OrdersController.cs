using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using QualityCapsIrina.Data;
using QualityCapsIrina.Models;


namespace QualityCapsIrina.Controllers
{    
    public class OrdersController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly StoreContext _context;
        private readonly ShoppingCart _shoppingCart;

        public OrdersController(StoreContext context, ShoppingCart shoppingCart, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _shoppingCart = shoppingCart;
            _userManager = userManager;
        }

        // GET: Orders
        //public async Task<IActionResult> Index()
        //{
        //    var storeContext = _context.Orders.Include(o => o.Customer);
        //    return View(await storeContext.ToListAsync());
        //}

        //// GET: Orders/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var order = await _context.Orders
        //        .Include(o => o.Customer)
        //        .SingleOrDefaultAsync(m => m.OrderID == id);
        //    if (order == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(order);
        //}

        // GET: Orders/Checkout
        public async Task<IActionResult> CheckOut(int step = 1)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account", new { returnUrl = $"/Orders/Checkout?step={step}" });

            if (!User.IsInRole("customer"))
            {
                TempData["Message"] = "Only customers can make orders";
                TempData["Title"] = "Error";
                return RedirectToAction("Index", "Home");
            }       
           
            if (step==1)
            {
                return RedirectToAction(nameof(ConfirmDetails));
            }
            else if (step==2)
            {
                var items = _shoppingCart.GetShoppingCartItems();
                _shoppingCart.ShoppingCartItems = items;
                var _order = new Order
                {
                    Status = OrderStatus.Waiting,
                    Date = DateTime.Now,
                    Subtotal = (double)_shoppingCart.GetShoppingCartTotal(),
                    GST = 15,
                    GrandTotal = Math.Round((double)_shoppingCart.GetShoppingCartTotal() * 1.15, 2),
                    CustomerID = _userManager.GetUserId(User)
                };

                if (ModelState.IsValid)
                {
                    _order.OrderItems = new List<OrderItem>();
                    foreach (var item in _shoppingCart.ShoppingCartItems)
                    {
                        var orderItem = new OrderItem
                        {
                            ItemID = item.Item.ItemId,
                            Quantity = item.Amount
                        };
                        _context.OrderItems.Add(orderItem);
                        _order.OrderItems.Add(orderItem);
                    }
                    _context.Orders.Add(_order);
                    await _context.SaveChangesAsync();
                    _shoppingCart.ClearCart();
                    return RedirectToAction(nameof(CheckoutComplete));
                    //return View(_order);
                }
            }            
            return RedirectToAction("Error","Home");
        }

        [Authorize]
        public async Task<IActionResult> ConfirmDetails()
        {
            Customer customer = (Customer)(await _userManager.GetUserAsync(User));
            return View(customer);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDetails(
            [Bind("FirstName, LastName, HomeNumber, MobileNumber, WorkNumber, AddressLine1, AddressLine2, City, ZipCode")] Customer submittedCustomer)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    Customer customer = (Customer)(await _userManager.GetUserAsync(User));
                    customer.FirstName = submittedCustomer.FirstName;
                    customer.LastName = submittedCustomer.LastName;
                    customer.HomeNumber = submittedCustomer.HomeNumber;
                    customer.MobileNumber = submittedCustomer.MobileNumber;
                    customer.WorkNumber = submittedCustomer.WorkNumber;
                    customer.AddressLine1 = submittedCustomer.AddressLine1;
                    customer.AddressLine2 = submittedCustomer.AddressLine2;
                    customer.City = submittedCustomer.City;
                    customer.ZipCode = submittedCustomer.ZipCode;                    
                    var result = await _userManager.UpdateAsync(customer);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Checkout", new { step = 2 });
                    }
                    else
                    {
                        return RedirectToAction("Error", "Home");
                    }
                }
                catch (InvalidOperationException e) {
                    return RedirectToAction("Error", "Home");
                }
                
            }
            return View(submittedCustomer);
        }

        // POST: Orders/Checkout
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> CheckOut(Order order)
        //{
        //    var items = _shoppingCart.GetShoppingCartItems();
        //    _shoppingCart.ShoppingCartItems = items;
        //    var _order = new Order
        //    {
        //        Status = OrderStatus.Waiting,
        //        Date = DateTime.Now,
        //        Subtotal = (double)_shoppingCart.GetShoppingCartTotal(),
        //        GST = 15,
        //        GrandTotal = (double)_shoppingCart.GetShoppingCartTotal() * 1.15,
        //        CustomerID = _userManager.GetUserId(User)
        //    };            

        //    if (ModelState.IsValid)
        //    {
        //        foreach (var item in _shoppingCart.ShoppingCartItems)
        //        {
        //            var orderItem = new OrderItem
        //            {
        //                ItemID = item.Item.ItemId,
        //                Quantity = item.Amount
        //            };
        //            _context.Add(orderItem);
        //            _order.OrderItems.Add(orderItem);
        //        }
        //        _context.Add(order);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(CheckOutComplete));
        //    }
            
        //    return View();
        //}        

        // GET: Orders/CheckOutComplete
        public IActionResult CheckoutComplete()
        {
            return View();
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderID == id);
        }
    }
}
