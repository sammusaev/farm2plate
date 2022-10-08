using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using farm2plate.Areas.Identity.Data;
using farm2plate.Models;
using Microsoft.AspNet.Identity;

namespace farm2plate.Controllers
{
    [Authorize(Roles="Customer")]
    public class CustomerController : Controller
    {
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;
        // private ApplicationUser _user;
        private readonly ApplicationDbContext _context;

        public CustomerController(Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager, ApplicationDbContext context) {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> ViewShop(int? ShopID)
        {
            //System.Diagnostics.Debug.WriteLine($"Shop ID {ShopID}");
            var shop = await _context.Shops.FindAsync(ShopID);
            //System.Diagnostics.Debug.WriteLine($"Just Shop {shop}");
            await _context.Entry(shop).Collection(x => x.Products).LoadAsync();
            //System.Diagnostics.Debug.WriteLine($"Shop Name {shop.Name}");
            return View(shop);
        }

        public async Task<IActionResult> ViewOrders()
        {
            var user = await _userManager.FindByIdAsync(User.Identity.GetUserId());
            System.Diagnostics.Debug.WriteLine($"USER ID {user.Id}");
            await _context.Entry(user).Collection(x => x.SOrders).LoadAsync();
            ViewBag.orders = user.SOrders;
            var ordersCount = user.SOrders.Count;
            System.Diagnostics.Debug.WriteLine($"!!!! SOrders {user.SOrders} SOrders.Count {user.SOrders.Count}");
            if (ordersCount > 0)
            {
                ViewBag.hasOrders = true;
            } else ViewBag.hasOrders = false;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Order([Bind("ProductQuantity")] SOrder sorder, int? ProductQuantity, int? ProductID, int? ShopID)
        {
            var user = await _userManager.FindByIdAsync(User.Identity.GetUserId());
            var product = await _context.Products.FindAsync(ProductID);
            sorder.ProductID = (int)ProductID;
            sorder.ProductQuantity = (int)ProductQuantity;
            sorder.ShopID = (int)ShopID;
            sorder.UserID = user.Id;
            sorder.SOrderStatus = Status.IN_PROGRESS;

            System.Diagnostics.Debug.WriteLine($"!!! ProductQuantity {ProductQuantity} ProductID {ProductID} ShopID {ShopID} sorderUserID {sorder.UserID}");

            _context.SOrders.Add(sorder);

            product.ProductQuantity = (int)(product.ProductQuantity - ProductQuantity);

             await _context.SaveChangesAsync();


            //System.Diagnostics.Debug.WriteLine($"Product Name {product.ProductName}");
            return View(); // order page
        }

        public async Task<IActionResult> Index()
        {
            var shops = _context.Shops;
            ViewBag.Shops = shops;
            return View();
        }

        /*public LocalRedirectResult LocalRedirect()
        {
            return LocalRedirect("/customer");
        }*/ // same as RedirectCustomerHome

        public RedirectToActionResult RedirectCustomerHome()
        {
            return RedirectToAction("Index", "Customer");
        }
    }
}
