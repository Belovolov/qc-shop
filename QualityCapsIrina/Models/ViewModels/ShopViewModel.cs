using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QualityCapsIrina.Models;
using QualityCapsIrina.Data;

namespace QualityCapsIrina.Models.ViewModels
{
    public class ShopViewModel
    {
        public PaginatedList<Item> items;
        public ICollection<Category> categories;
        public SortOrder sortOrder;
        public double MaxPrice {
            get
            {
                return items.Max(i => i.Price);
            }
        }
        
        public double MinPrice {
            get
            {
                return items.Min(i => i.Price);
            }
        }
    }
}
