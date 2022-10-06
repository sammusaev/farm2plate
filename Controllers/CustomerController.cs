using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using farm2plate.Areas.Identity.Data;
using farm2plate.Models;


namespace farm2plate.Controllers
{
    [Authorize(Roles="Customer")]
    public class CustomerController : Controller
    {
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;
        private ApplicationUser _user;
        private readonly ApplicationDbContext _context;

        public CustomerController(ApplicationDbContext context) {
            _context = context;
        }

        public async Task<IActionResult> ViewShop(int ?ID)
        {
            System.Diagnostics.Debug.WriteLine($"Shop {ID}");
            var shop = await _context.Shops.FindAsync(ID);
            System.Diagnostics.Debug.WriteLine($"Shop {shop.Name}");
            return View(shop);
            //return RedirectToAction("Index", "FlowerList");
        }

        public async Task<IActionResult> Index()
        {
            var shops = _context.Shops;
            ViewBag.Shops = shops;
            return View();
        }
     }
}
