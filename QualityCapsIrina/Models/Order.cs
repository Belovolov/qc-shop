using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace QualityCapsIrina.Models
{
    public class Order
    {
        [Display(Name = "Order ID")]
        public int OrderID  { get; set; }

        public OrderStatus Status  { get; set; }
        [Display(Name = "Order Date")]
        public DateTime Date { get; set; }
        public double Subtotal { get; set; }
        public double GST  { get; set; }
        [Display(Name = "Grand Total")]
        public double GrandTotal { get; set; }
        public string CustomerID { get; set; }
        public Customer Customer { get; set; }        
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
