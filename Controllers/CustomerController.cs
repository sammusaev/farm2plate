using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using farm2plate.Areas.Identity.Data;
using farm2plate.Models;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System;
using farm2plate.AWServices;

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
            ViewBag.BucketName = Environment.GetEnvironmentVariable("AWS_IMAGE_BUCKET_NAME");
            return View(shop);
        }

        public async Task<IActionResult> ViewOrders()
        {
            var user = await _userManager.FindByIdAsync(User.Identity.GetUserId());

            var productQuantitiesList = new List<double>();
            var orderStatusesList = new List<Status>();
            var orderIdsList = new List<int>();

            System.Diagnostics.Debug.WriteLine($"USER ID {user.Id}");
            await _context.Entry(user).Collection(x => x.SOrders).LoadAsync();

            foreach (var order in user.SOrders)
            {
                productQuantitiesList.Add(order.ProductQuantity);
                orderStatusesList.Add(order.SOrderStatus);
                orderIdsList.Add(order.SOrderID);
            }

            ViewBag.orders = user.SOrders;
            var ordersCount = user.SOrders.Count;
            //System.Diagnostics.Debug.WriteLine($"!!!! SOrders {user.SOrders} SOrders.Count {user.SOrders.Count}");

            var productNamesList = new List<string>();
            var productPricesList = new List<double>();
            var productImagesList = new List<string>();
            var shopNamesList = new List<string>();
            var totalPricesList = new List<double>();

            foreach (var order in user.SOrders) {
                var product = await _context.Products.FindAsync(order.ProductID);
                var shop = await _context.Shops.FindAsync(order.ShopID);
                productNamesList.Add(product.ProductName);
                productImagesList.Add(product.ProductImage);
                productPricesList.Add(order.SorderUnitPrice);
                shopNamesList.Add(shop.ShopName);
                totalPricesList.Add(order.SorderAmount);
            }

            var productNames = productNamesList.ToArray();
            var productPrices = productPricesList.ToArray();
            var productImages = productImagesList.ToArray();
            var shopNames = shopNamesList.ToArray();
            var totalPrices = totalPricesList.ToArray();
            var productQuantities = productQuantitiesList.ToArray();
            var orderStatuses = orderStatusesList.ToArray();
            var orderIdS = orderIdsList.ToArray();

            ViewBag.productNames = productNames;
            ViewBag.productPrices = productPrices;
            ViewBag.productImages = productImages;
            ViewBag.shopNames = shopNames;
            ViewBag.totalPrices = totalPrices;
            ViewBag.productQuantities = productQuantities;
            ViewBag.orderStatuses = orderStatuses;
            ViewBag.orderIdS = orderIdS;

            ViewBag.BucketName = Environment.GetEnvironmentVariable("AWS_IMAGE_BUCKET_NAME");

            if (ordersCount > 0) {
                ViewBag.hasOrders = true;
            } else ViewBag.hasOrders = false;
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ChangeOrderStatus([Bind("SOrderStatus")] SOrder sorder, int? OrderID, Status SOrderStatus)
        {
            string CustomerPhoneNumber = _context.Users.FindAsync(sorder.UserID).Result.PhoneNumber;
            SNSService service = new SNSService();
            service.SendSMS(CustomerPhoneNumber, $"Order {OrderID} is {SOrderStatus.ToString()}");

            var order = await _context.SOrders.FindAsync(OrderID);
            ViewBag.OldOrderStatus = order.SOrderStatus;
            order.SOrderStatus = SOrderStatus;
            await _context.SaveChangesAsync();

            ViewBag.OrderID = OrderID;
            ViewBag.NewOrderStatus = SOrderStatus;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Order([Bind("ProductQuantity")] SOrder sorder, double? ProductQuantity, int? ProductID, int? ShopID, string ShopName, string ProductImage, double? ProductPrice)
        {

            System.Diagnostics.Debug.WriteLine($"!!! ProductQuantity {ProductQuantity} ProductID {ProductID} ShopID {ShopID} sorderUserID {sorder.UserID}");

            var user = await _userManager.FindByIdAsync(User.Identity.GetUserId());
            var product = await _context.Products.FindAsync(ProductID);
            sorder.ProductID = (int)ProductID;
            sorder.ProductQuantity = (double)ProductQuantity;
            sorder.ShopID = (int)ShopID;
            sorder.UserID = user.Id;
            sorder.SOrderStatus = Status.IN_PROGRESS;
            sorder.SorderAmount = (double)(ProductQuantity * ProductPrice);
            sorder.SorderUnitPrice = (double)ProductPrice;

            ViewBag.ShopName = ShopName;
            ViewBag.ShopName = ShopName;
            ViewBag.BucketName = Environment.GetEnvironmentVariable("AWS_IMAGE_BUCKET_NAME");
            ViewBag.ProductImage = ProductImage;
            ViewBag.ProductPrice = ProductPrice;
            ViewBag.TotalPrice = sorder.SorderAmount;    

            _context.SOrders.Add(sorder);

            product.ProductQuantity = (double)(product.ProductQuantity - ProductQuantity);
            await _context.SaveChangesAsync();
            return View(sorder);
        }

        public async Task<IActionResult> Index()
        {
            var _user = await _userManager.FindByIdAsync(User.Identity.GetUserId());

            bool phoneExists = false;
            if (_user.PhoneNumber != null)
                phoneExists = true;
            bool verified = _user.PhoneIsVerified;

            ViewBag.PhoneExists = phoneExists;
            ViewBag.PhoneIsVerified = verified;

            var shops = _context.Shops;
            ViewBag.Shops = shops;
            return View();
        }

        public IActionResult VerifyPhoneNumberView()
        {
            return View();
        }

        public async Task<IActionResult> VerifyPhoneNumber(string token)
        {
            var _user = await _userManager.FindByIdAsync(User.Identity.GetUserId());
            string number = _user.PhoneNumber;
            SNSService service = new SNSService();
            await service.ConfirmSandbox(token, number);
            if (await service.IsNumberConfirmed(number)) {
                _user.PhoneIsVerified = true;
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Customer");
            }
            ViewBag.Error = "Invalid/Expired token!";
            return RedirectToAction("VerifyPhoneNumberView", "Customer");
        }

        public string formatNumber(string number)
        {
            if (!number.Contains("+6"))
                return "+6" + number;
            return number;
        }

        public async Task<IActionResult> AddPhoneNumber(string PhoneNumber)
        {
            var _user = await _userManager.FindByIdAsync(User.Identity.GetUserId());
            SNSService service = new SNSService();
            PhoneNumber = formatNumber(PhoneNumber);
            _user.PhoneNumber = PhoneNumber;
            await _context.SaveChangesAsync();
            service.AddToSandbox(PhoneNumber);
            service.AddSubscriberSMS(PhoneNumber);
            return RedirectToAction("Index", "Customer");
        }
        public RedirectToActionResult RedirectCustomerHome()
        {
            return RedirectToAction("Index", "Customer");
        }

        public LocalRedirectResult RedirectViewOrders()
        {
            return LocalRedirect("/Customer/ViewOrders");
        }
    }
}
