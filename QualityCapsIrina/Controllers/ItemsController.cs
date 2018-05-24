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
        [Route("[controller]/men")]
        [Route("[controller]/women")]
        public async Task<IActionResult> Shop(
            double minPrice,
            double maxPrice,
            int category,
            string searchQuery,
            int? page,
            int pageSize = 6,
            SortOrder sortOrder = SortOrder.Name)
        {
            string genderString = Request.Path.Value.Split('/')[2].ToLower();
            Gender gender = (genderString == "men") ? Gender.Men : Gender.Women;
            ViewData["Gender"] = gender;

            var shopViewModel = new ShopViewModel();
            IQueryable<Item> items = _context.Items
                .Where(item => (item.Gender == gender) || (item.Gender == Gender.Unisex))
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

            //string search
            ViewData["SearchQuery"] = searchQuery;
            if (searchQuery != null && searchQuery.Length > 0)
            {
                items = items.Where(i => i.Name.Contains(searchQuery));
            }

            //sorting
            ViewData["SortOrders"] = GenerateSortList(sortOrder);
            ViewData["CurrentSortOrder"] = sortOrder;
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
                case SortOrder.Name:
                default:
                    items = items.OrderBy(i => i.Name);
                    break;
            }

            //shopViewModel.items = await items.ToListAsync();
            //pagination
            ViewData["PageSizes"] = new SelectList(new List<int> { 6, 12, 20, 40 }, pageSize);
            ViewData["PageSize"] = pageSize;
            shopViewModel.items = await PaginatedList<Item>.CreateAsync(items.AsNoTracking(), page ?? 1, pageSize);

            shopViewModel.categories = await _context.Categories
                    .Where(x => x.Items.Any<Item>(item => (item.Gender == gender) || (item.Gender == Gender.Unisex)))
                    .ToListAsync();
            shopViewModel.categories.Add(new Category { CategoryId = 0, Name = "All" });

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
