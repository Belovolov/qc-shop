using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QualityCapsIrina.Models
{
    public class OrderItem
    {
        public int OrderID { get; set; }
        public Order Order { get; set; }
        public int ItemID { get; set; }
        public Item Item { get; set; }
        public int Quantity { get; set; }
    }
}
