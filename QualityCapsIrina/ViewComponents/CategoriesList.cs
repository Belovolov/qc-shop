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
    public class CategoriesList : ViewComponent
    {
        private readonly StoreContext db;
        public CategoriesList(StoreContext context)
        {
            db = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(Gender gender)
        {
            var action = "";
            switch (gender)
            {
                case Gender.Men:
                    action = "men";
                    break;
                case Gender.Women:
                    action = "women";
                    break;
            }
            var categories = await GetCategoriesAsync(gender);

            var model = new CategoriesListViewModel(categories, action);
            
            //categories.Prepend(new Category { Name = "All", CategoryId = 0 });
            return View(model);
        }
        private Task<List<Category>> GetCategoriesAsync(Gender gender)
        {
            return db.Categories
                    .Where(x => x.Items.Any<Item>(item=>(item.Gender==gender)||(item.Gender == Gender.Unisex)))
                    .ToListAsync();            
        }
    }
}
