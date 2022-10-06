using farm2plate.Areas.Identity.Data;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace farm2plate.Controllers
{
    [Authorize(Roles="Vendor")]
    public class VendorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
