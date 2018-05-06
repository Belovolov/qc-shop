using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QualityCapsIrina.Models
{
    public class Order
    {
        public int OrderID  { get; set; }
        public OrderStatus Status  { get; set; }
        public DateTime Date { get; set; }
        public double Subtotal { get; set; }
        public double GST  { get; set; }
        public int CustomerID { get; set; }
        public Customer Customer { get; set; }        
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
