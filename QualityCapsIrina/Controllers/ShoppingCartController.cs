using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QualityCapsIrina.Data;
using QualityCapsIrina.Models;
using QualityCapsIrina.Models.ViewModels;

namespace QualityCapsIrina.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly StoreContext _context;
        private readonly ShoppingCart _shoppingCart;

        public ShoppingCartController(StoreContext context, ShoppingCart shoppingCart)
        {
            _context = context;
            _shoppingCart = shoppingCart;
        }

        public ViewResult Index()
        {
            var items = _shoppingCart.GetShoppingCartItems();
            _shoppingCart.ShoppingCartItems = items;

            var shoppingCartViewModel = new ShoppingCartViewModel
            {
                ShoppingCart = _shoppingCart,
                ShoppingCartCount = _shoppingCart.GetShoppingCartCount(),
                ShoppingCartTotal = _shoppingCart.GetShoppingCartTotal()
            };

            return View(shoppingCartViewModel);
        }

        public RedirectToActionResult AddToShoppingCart(int itemId, int amount=1)
        {
            var selectedItem = _context.Items.FirstOrDefault(p => p.ItemId == itemId);

            if (selectedItem != null && amount >=1)
            {
                _shoppingCart.AddToCart(selectedItem, amount);
            }
            return RedirectToAction("Index");
        }
        public RedirectToActionResult SetAmountInCart(int itemId, int? amount)
        {
            var selectedItem = _context.Items.FirstOrDefault(p => p.ItemId == itemId);
            if (amount!=null)
            {
                _shoppingCart.SetAmountInCart(selectedItem, (int)amount);
            }            
            return RedirectToAction("Index");
        }

        public RedirectToActionResult RemoveFromShoppingCart(int itemId)
        {
            var selectedItem = _context.Items.FirstOrDefault(p => p.ItemId == itemId);

            if (selectedItem != null)
            {
                _shoppingCart.RemoveFromCart(selectedItem);
            }
            return RedirectToAction("Index");
        }
        public RedirectToActionResult ClearShoppingCart()
        {
            _shoppingCart.ClearCart();
            return RedirectToAction("Index");
        }
    }
}