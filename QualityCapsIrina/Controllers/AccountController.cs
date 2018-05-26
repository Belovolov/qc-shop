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
            IEmailSender emailSender)
        {
            _userManager = userManager;
            var provider = new EmailTokenProvider<IdentityUser>();
            _userManager.RegisterTokenProvider("Default", provider);

            _signInManager = signInManager;
            _emailSender = emailSender;
        }
        [HttpGet]
        public IActionResult Register(string returnUrl = null)
        {
            return View(new RegisterViewModel { ReturnUrl = returnUrl });
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                Customer user = new Customer
                {
                    Email = model.Email,
                    UserName = (model.Username != null) ? model.Username : model.Email,
                    IsLocked = false
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
                var result =
                    await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
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
        public async Task<IActionResult> Profile()
        {
            Customer user = (Customer) await _userManager.GetUserAsync(User);            
            return View(user);            
            //_signInManager.
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "customer")]
        public async Task<IActionResult> Update([Bind("Id, FirstName, LastName, AddressLine1, AddressLine2, City, ZipCode")] Customer customer)
        {
            var user = await _userManager.GetUserAsync(User);
            if (ModelState.IsValid && user.Id == customer.Id)
            {
                try
                {
                    await _userManager.UpdateAsync(customer);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if ((await _userManager.FindByIdAsync(customer.Id)) == null)
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
            return View(user);            
        }
        [HttpGet]
        public async Task<IActionResult> Orders()
        {
            Customer user = (Customer)await _userManager.GetUserAsync(User);
            if (user!=null)
            {
                ICollection<Order> orders = (user.Orders != null) ? user.Orders : new List<Order>();
                return View(orders);
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }
        }
    }
}