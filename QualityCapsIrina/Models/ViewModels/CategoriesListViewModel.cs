using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QualityCapsIrina.Models;
using QualityCapsIrina.Data;

namespace QualityCapsIrina.Models.ViewModels
{
    public class CategoriesListViewModel
    {
        public List<Category> categories;
        public string action;

        public CategoriesListViewModel(List<Category> categories, string action)
        {
            this.categories = categories ?? throw new ArgumentNullException(nameof(categories));
            this.action = action ?? throw new ArgumentNullException(nameof(action));
        }
    }
}
