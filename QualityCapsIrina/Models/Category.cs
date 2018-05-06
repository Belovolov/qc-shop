using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QualityCapsIrina.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public ICollection<Item> Items { get; set; }
    }
}
