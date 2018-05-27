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

namespace QualityCapsIrina.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles="admin")]
    public class CustomersController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly StoreContext _context;

        public CustomersController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager, 
            StoreContext context)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        // GET: Customers
        public async Task<ActionResult> Index(bool? isLocked)
        {
            var customers = _context.Customers.AsQueryable();
            if (isLocked!=null)
            {
                customers = customers.Where(o => o.IsLocked == isLocked);
            }
            var statusList = new List<SelectListItem> {
                new SelectListItem {Text = "All", Value = null, Selected = (isLocked==null)},
                new SelectListItem {Text = "Only locked", Value = "True", Selected = (isLocked == true)},
                new SelectListItem {Text = "Only unlocked", Value = "False", Selected = (isLocked==false)}
            };
            ViewData["StatusOptions"] = statusList;
            
            customers.OrderBy(c => c.FirstName);
            return View(customers);
        }
        public async Task<IActionResult> Lock(string id)
        {
            return await SetUserLocked(id, true);
        }
        public async Task<IActionResult> Unlock(string id)
        {
            return await SetUserLocked(id, false);
        }
        private async Task<IActionResult> SetUserLocked(string id, bool isLocked)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.SingleOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }
            customer.IsLocked = isLocked;
            _context.Update(customer);
            await _context.SaveChangesAsync();  
            if (isLocked)
            {
                TempData["Message"] = $"Customer {customer.Id} with email {customer.Email} has been locked";
                TempData["Title"] = "User locked";
            }
            else
            {
                TempData["Message"] = $"Customer {customer.Id} with email {customer.Email} has been unlocked";
                TempData["Title"] = "User unlocked";
            }
            return RedirectToAction(nameof(Index));
        }
        // GET: Customers/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .Include(c => c.Orders)
                .SingleOrDefaultAsync(m => m.Id == id);


            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }        

        // GET: Customers/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.SingleOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }            
            return View(customer);
        }

        // POST: Customers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,FirstName, LastName, MobileNumber, WorkNumber, HomeNumber, AddressLine1, AddressLine2, City, ZipCode")] Customer submittedCustomer)
        {
            if (id != submittedCustomer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                Customer customer = (Customer)(await _userManager.FindByIdAsync(id));
                try
                {
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
                    await _userManager.UpdateAsync(customer);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(submittedCustomer.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
           
            return View(submittedCustomer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .Include(c => c.Orders)
                .SingleOrDefaultAsync(c => c.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var customer = await _context.Customers
                .Include(c => c.Orders)
                .SingleOrDefaultAsync(c => c.Id == id);
            try
            {
                _context.Orders.RemoveRange(customer.Orders);
                await _userManager.DeleteAsync(customer);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToAction("Error", "Home");
            }
            int wOrders = customer.Orders.Where(o => o.Status == OrderStatus.Waiting).Count();
            TempData["Title"] = "Deleted";
            TempData["Message"] = $"Customer {customer.Email} with {wOrders} waiting orders a was deleted!";
            return RedirectToAction(nameof(Index));
        }
        private bool UserExists(string id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
}