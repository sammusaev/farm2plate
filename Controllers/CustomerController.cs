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

        public async Task<IActionResult> ViewShop(int? ShopID)
        {
            System.Diagnostics.Debug.WriteLine($"Shop ID {ShopID}");
            var shop = await _context.Shops.FindAsync(ShopID);
            System.Diagnostics.Debug.WriteLine($"Just Shop {shop}");
            await _context.Entry(shop).Collection(x => x.Products).LoadAsync();
            System.Diagnostics.Debug.WriteLine($"Shop Name {shop.Name}");
            return View(shop);
            //return RedirectToAction("Index", "FlowerList");
        }

        [HttpPost]
        public async Task<IActionResult> Order([Bind("ProductQuantity")] SOrder sorder, int? ProductQuantity, int? ProductId)
        {
           System.Diagnostics.Debug.WriteLine($"ProductQuantity {ProductQuantity}");
            var product = await _context.Products.FindAsync(ProductId);
            System.Diagnostics.Debug.WriteLine($"product {product}");
            //_context.SOrder.Add(product);
            //await _context.SaveChangesAsync();
            System.Diagnostics.Debug.WriteLine($"Product Name {product.ProductName}");
            return View(); // order page
        }

        public async Task<IActionResult> Index()
        {
            var shops = _context.Shops;
            ViewBag.Shops = shops;
            return View();
        }
     }
}
