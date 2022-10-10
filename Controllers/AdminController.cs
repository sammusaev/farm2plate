using farm2plate.Areas.Identity.Data;
using farm2plate.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace farm2plate.Controllers
{
    [Authorize(Roles="Admin")]
    public class AdminController : Controller
    {
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;
        // private ApplicationUser _user;
        private readonly ApplicationDbContext _context;
        public AdminController(Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> ViewAllOrder()
        {

            var orders = _context.SOrders;
            ViewBag.SOrders = orders.ToArray();
            System.Diagnostics.Debug.WriteLine($"!!!! SOrders {orders} ");

            int ordersCount = orders.Count();
            ViewBag.ordersCount = ordersCount;


            var productNamesList = new List<string>();
            var shopNamesList = new List<string>();

            foreach (var order in orders)
            {
                Product product = await _context.Products.FindAsync(order.ProductID);
                Shop shop = await _context.Shops.FindAsync(order.ShopID);
                productNamesList.Add(product.ProductName);
                shopNamesList.Add(shop.ShopName);
            }
            System.Diagnostics.Debug.WriteLine($"!!!! SOrders {productNamesList} ");
            System.Diagnostics.Debug.WriteLine($"!!!! SOrders {shopNamesList} ");

            ViewBag.productNames = productNamesList;
            ViewBag.shopNames = shopNamesList;

            return View();
        }

        public IActionResult ViewAllShop()
        {
            var shops = _context.Shops;
            ViewBag.Shops = shops;
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
