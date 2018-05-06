using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace QualityCapsIrina.Models
{
    public class Supplier
    {
        public int SupplierId { get; set; }
        public string Name { get; set; }

        [Phone(ErrorMessage = "Incorrect Phone Format")]
        public string HomeNumber { get; set; }

        [Phone(ErrorMessage = "Incorrect Phone Format")]
        public string WorkNumber { get; set; }

        [Phone(ErrorMessage = "Incorrect Phone Format")]
        public string MobileNumber { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email")]
        public string Email { get; set; }

        public string Address { get; set; }
        public ICollection<Item> Items {get;set;}
    }
}
