﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace QualityCapsIrina.Models
{
    public class Customer : IdentityUser
    {
        [Required(ErrorMessage = "Please enter your first name")]
        [Display(Name = "First name")]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter your last name")]
        [Display(Name = "Last name")]
        [StringLength(50)]
        public string LastName { get; set; }

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

        public bool IsLocked { get; set; }

        public bool IsNotAllowed { get
            {
                return IsLocked;
            }
        }

        public ICollection<Order> Orders { get; set; }

        public string FullName { get
            {
                return $"{FirstName} {LastName}";
            }
        }

        public string FullAddress { get
            {
                StringBuilder address = new StringBuilder();
                address.Append(EmptyOrNull(AddressLine1) ? "" : AddressLine1 + ", ");
                address.Append(EmptyOrNull(AddressLine2) ? "" : AddressLine2 + ", ");
                address.Append(EmptyOrNull(City) ? "" : City + ", ");
                address.Append(EmptyOrNull(ZipCode) ? "" : ZipCode + ", ");
                return address.ToString().TrimEnd(new char[] { ',', ' ' });
            }
        }
        private bool EmptyOrNull(string s)
        {
            return (s == null) || (s.Length == 0);
        }
    }
}
