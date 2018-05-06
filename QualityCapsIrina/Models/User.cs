using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace QualityCapsIrina.Models
{
    public class User : IdentityUser
    {        
        public Customer Customer { get; set; }
    }
}
