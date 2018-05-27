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
        [Display(Name = "Home number")]
        public string HomeNumber { get; set; }

        [Phone(ErrorMessage = "Incorrect Phone Format")]
        [Display(Name = "Home number")]
        public string WorkNumber { get; set; }

        [Phone(ErrorMessage = "Incorrect Phone Format")]
        [Display(Name = "Mobile number")]
        public string MobileNumber { get; set; }

        [Required(ErrorMessage = "Please enter your address")]
        [StringLength(100)]
        [Display(Name = "Address Line 1")]
        public string AddressLine1 { get; set; }

        [Display(Name = "Address Line 2")]
        public string AddressLine2 { get; set; }

        [Required(ErrorMessage = "Please enter your city")]
        [StringLength(50)]
        public string City { get; set; }

        [Required(ErrorMessage = "Please enter your zip code")]
        [Display(Name = "Zip code")]
        [StringLength(10, MinimumLength = 4)]
        public string ZipCode { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email")]
        public string Email { get; set; }
        
        public ICollection<Item> Items {get;set;}
    }
}
