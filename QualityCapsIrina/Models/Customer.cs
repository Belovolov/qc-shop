using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace QualityCapsIrina.Models
{
    public class Customer
    {
        public int CustomerID { get; set; }
        public string Name { get; set; }

        [Phone(ErrorMessage = "Incorrect Phone Format")]
        public string HomeNumber { get; set; }

        [Phone(ErrorMessage = "Incorrect Phone Format")]
        public string WorkNumber { get; set; }

        [Phone(ErrorMessage = "Incorrect Phone Format")]
        public string MobileNumber { get; set; }

        public string Address { get; set; }
        public int UserId { get; set;}
        public User User { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
