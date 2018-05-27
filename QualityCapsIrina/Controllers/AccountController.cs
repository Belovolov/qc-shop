using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using QualityCapsIrina.Services;
using QualityCapsIrina.Data;
using QualityCapsIrina.Models;
using QualityCapsIrina.Models.ViewModels;

namespace QualityCapsIrina.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private StoreContext _context;
        private IEmailSender _emailSender;

        public AccountController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            StoreContext context,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            var provider = new EmailTokenProvider<IdentityUser>();
            _userManager.RegisterTokenProvider("Default", provider);

            _signInManager = signInManager;
            _emailSender = emailSender;
            _context = context;
        }
        [HttpGet]
        public IActionResult Register(string returnUrl = null)
        {
            return View(new RegisterViewModel { ReturnUrl = returnUrl });
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            bool IsEmailInUse = _userManager.Users.Any(u => u.Email == model.Email);
            if (IsEmailInUse == true)
            {
                ModelState.AddModelError("Email", "Email already in use");
            }
            if (ModelState.IsValid)
            {
                Customer user = new Customer
                {
                    Email = model.Email,
                    UserName = (model.Username != null) ? model.Username : model.Email,
                    IsLocked = false,
                    WorkNumber = (model.PhoneType == "work") ? model.PhoneNumber : null,
                    HomeNumber = (model.PhoneType == "home") ? model.PhoneNumber : null,
                    MobileNumber = (model.PhoneType == "mobile") ? model.PhoneNumber : null
                };
                //adding user
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //add customer role
                    await _userManager.AddToRoleAsync(user, "customer");
                    //send confirmation letter                    
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code }, Request.Scheme);
                    await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);

                    // setting cookie
                    await _signInManager.SignInAsync(user, false);
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            if (string.IsNullOrEmpty(returnUrl) && Request.Headers["Referer"].ToString() != null)
                returnUrl = new Uri(Request.Headers["Referer"].ToString()).PathAndQuery;
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                //try treat as username
                var username = model.EmailOrUsername;
                var result =
                    await _signInManager.PasswordSignInAsync(username, model.Password, model.RememberMe, false);                
                if (!result.Succeeded)
                {
                    //try treat as email
                    var email = model.EmailOrUsername;
                    var userByEmail = await _userManager.FindByEmailAsync(email);
                    if (userByEmail != null)
                    {
                        result =
                            await _signInManager.PasswordSignInAsync(userByEmail, model.Password, model.RememberMe, false);
                    }                    
                }
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Incorrect login/password");
                }
            }
            return View(model);
        }

        [HttpGet]

        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user.EmailConfirmed)
            {
                TempData["Title"] = "Info";
                TempData["Message"] = "Your email is already confirmed";
            }
            else
            {
                var result = await _userManager.ConfirmEmailAsync(user, code);
                if (result.Succeeded)
                {
                    TempData["Title"] = "Success";
                    TempData["Message"] = "Your email was successfully confirmed";
                }
                else
                {
                    TempData["Title"] = "Error";
                    TempData["Message"] = "Email confirmation failed. Invalid token";
                }
            }

            return RedirectToAction("Index", "Home");
        }
        
        [HttpGet]
        [Authorize(Roles = "customer")]
        public async Task<IActionResult> Profile(bool edit = false)
        {
            Customer user = (Customer) await _userManager.GetUserAsync(User);
            ViewData["Edit"] = edit;
            return View(user);            
            //_signInManager.
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "customer")]
        public async Task<IActionResult> Update(
            [Bind("Id, FirstName, LastName, MobileNumber, WorkNumber, HomeNumber, AddressLine1, AddressLine2, City, ZipCode")] Customer submittedCustomer)
        {
            Customer customer = (Customer)(await _userManager.GetUserAsync(User));
            if (ModelState.IsValid && customer.Id == submittedCustomer.Id)
            {
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
                    if ((await _userManager.FindByIdAsync(submittedCustomer.Id)) == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Profile");
            }
            ViewData["Edit"] = true;
            return View("Profile", customer);
        }
        [HttpGet]
        public async Task<IActionResult> Orders()
        {
            Customer user = (Customer)await _userManager.GetUserAsync(User);
            
            if (user!=null)
            {
                var orders = await _context.Orders
                    .Where(o => o.CustomerID == user.Id)
                    .OrderByDescending(o => o.Date)
                    .ToListAsync();
                return View(orders);
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }
        }
    }
}