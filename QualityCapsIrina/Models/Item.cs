using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QualityCapsIrina.Models
{
    public class Item
    {
        public int ItemId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public string ImageUrl { get; set; }
        public Gender Gender { get; set; }
        public int SupplierId { get; set; }
        public int CategoryId { get; set; }
        public Supplier Supplier { get; set; }
        public Category Category { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
