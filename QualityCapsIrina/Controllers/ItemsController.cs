using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QualityCapsIrina.Data;
using QualityCapsIrina.Models;
using QualityCapsIrina.Models.ViewModels;

namespace QualityCapsIrina.Controllers
{
    public class ItemsController : Controller
    {
        private readonly StoreContext _context;

        public ItemsController(StoreContext context)
        {
            _context = context;
        }

        // GET: Items
        public async Task<IActionResult> Index()
        {
            var shopViewModel = new ShopViewModel();
            shopViewModel.items = await _context.Items
                .Include(i => i.Category).Include(i => i.Supplier).ToListAsync();
            shopViewModel.categories = await _context.Categories
                    .ToListAsync();
            return View(shopViewModel);
        }
        // GET: Items
        public async Task<IActionResult> Men(
            double minPrice,
            double maxPrice,
            int category,
            SortOrder sortOrder = SortOrder.Name
          )
        {
            var shopViewModel = new ShopViewModel();
            IQueryable<Item> items = _context.Items
                .Where(item => (item.Gender == Gender.Men) || (item.Gender == Gender.Unisex))
                .Include(i => i.Category).Include(i => i.Supplier);

            //category filtering
            ViewData["ActiveCategory"] = category;
            if (category != 0)
            {
                items = items.Where(i => i.CategoryId == category);
            }

            //price filtering  
            var interimItemList = items.ToList();
            ViewData["MinScalePrice"] = interimItemList.Min(i => i.Price);
            ViewData["MinPrice"] = (minPrice != 0) ? minPrice : ViewData["MinScalePrice"];
            ViewData["MaxScalePrice"] = interimItemList.Max(i => i.Price);
            ViewData["MaxPrice"] = (maxPrice != 0) ? maxPrice : ViewData["MaxScalePrice"];

            if (minPrice != 0)
            {
                items = items.Where(i => i.Price >= minPrice);
            }
            if (maxPrice != 0)
            {
                items = items.Where(i => i.Price <= maxPrice);
            }

            //sorting
            ViewData["SortOrders"] = GenerateSortList(sortOrder);
            switch (sortOrder)
            {
                case SortOrder.Price:
                    items = items.OrderBy(i => i.Price);
                    break;
                case SortOrder.PriceDesc:
                    items = items.OrderByDescending(i => i.Price);
                    break;
                case SortOrder.NameDesc:
                    items = items.OrderByDescending(i => i.Name);
                    break;
                case SortOrder.Name: default:
                    items = items.OrderBy(i => i.Name);
                    break;
            }

            shopViewModel.items = await items.ToListAsync();
            shopViewModel.categories = await _context.Categories
                    .Where(x => x.Items.Any<Item>(item => (item.Gender == Gender.Men) || (item.Gender == Gender.Unisex)))
                    .ToListAsync();
            shopViewModel.categories.Add(new Category { CategoryId = 0, Name = "All" });

            
            return View(shopViewModel);
        }
        // GET: Items
        public async Task<IActionResult> Women()
        {            
            var shopViewModel = new ShopViewModel();
            shopViewModel.items = await _context.Items
                .Where(item => (item.Gender == Gender.Women) || (item.Gender == Gender.Unisex))
                .Include(i => i.Category).Include(i => i.Supplier).ToListAsync();
            shopViewModel.categories = await _context.Categories
                    .Where(x => x.Items.Any<Item>(item => (item.Gender == Gender.Women) || (item.Gender == Gender.Unisex)))
                    .ToListAsync();
            return View(shopViewModel);
        }

        // GET: Items/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Items
                .Include(i => i.Category)
                .Include(i => i.Supplier)
                .SingleOrDefaultAsync(m => m.ItemId == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }
        
        private bool ItemExists(int id)
        {
            return _context.Items.Any(e => e.ItemId == id);
        }
        private List<SelectListItem> GenerateSortList(SortOrder selected)
        {
            var sortList = new List<SelectListItem>();
            foreach (SortOrder v in Enum.GetValues(typeof(SortOrder)))
            {
                sortList.Add(new SelectListItem
                {
                    Text = v.GetDescription(),
                    Value = v.ToString(),
                    Selected = (v == selected)
                });
            }
            return sortList;
        }
    }
}
