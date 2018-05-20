using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QualityCapsIrina.Data;
using QualityCapsIrina.Models;
using QualityCapsIrina.Models.ViewModels;

namespace QualityCapsIrina.ViewComponents
{
    public class LatestArrivals : ViewComponent
    {
        private StoreContext _context;
        public LatestArrivals(StoreContext context)
        {
            _context = context;
        }
        public IViewComponentResult Invoke()
        {
            var items = _context.Items.OrderBy(i => i.ArrivalDate).Take(10);
            return View(items);
        }
    }
}
