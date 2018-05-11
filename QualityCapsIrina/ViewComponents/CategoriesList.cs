using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QualityCapsIrina.Data;
using QualityCapsIrina.Models;

namespace QualityCapsIrina.ViewComponents
{
    public class CategoriesList : ViewComponent
    {
        private readonly StoreContext db;
        public CategoriesList(StoreContext context)
        {
            db = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(Gender gender)
        {
            var categories = await GetCategoriesAsync(gender);
            return View(categories);
        }
        private Task<List<Category>> GetCategoriesAsync(Gender gender)
        {
            return db.Categories
                    .Where(x => x.Items.Any<Item>(item=>(item.Gender==gender)||(item.Gender == Gender.Unisex)))
                    .ToListAsync();
            
        }
    }
}
