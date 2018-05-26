using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace QualityCapsIrina.Controllers
{
    public class CustomersController : Controller
    {
        // GET: Customers/Profile/5
        public ActionResult Profile(int id)
        {
            return View();
        }

        // GET: Customers/Create
        public ActionResult Orders()
        {
            return View();
        }        
    }
}