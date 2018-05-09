using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QualityCapsIrina.Data;
using QualityCapsIrina.Models;

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
            var storeContext = _context.Items.Include(i => i.Category).Include(i => i.Supplier);
            return View(await storeContext.ToListAsync());
        }
        // GET: Items
        public async Task<IActionResult> Men()
        {
            var storeContext = _context.Items
                .Where(item => (item.Gender == Gender.Men) || (item.Gender == Gender.Unisex))
                .Include(i => i.Category).Include(i => i.Supplier);
            return View(await storeContext.ToListAsync());
        }
        // GET: Items
        public async Task<IActionResult> Women()
        {
            var storeContext = _context.Items
                .Where(item => (item.Gender == Gender.Women) || (item.Gender == Gender.Unisex))
                .Include(i => i.Category).Include(i => i.Supplier);
            return View(await storeContext.ToListAsync());
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
    }
}
