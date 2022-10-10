using farm2plate.Areas.Identity.Data;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using farm2plate.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OAuth;
using farm2plate.AWServices;
using Microsoft.AspNetCore.Http;
using MANCI = Microsoft.AspNetCore.Identity;

namespace farm2plate.Controllers
{
    [Authorize(Roles="Vendor")]
    public class VendorController : Controller
    {
        private readonly MANCI.UserManager<ApplicationUser> _userManager;
        private ApplicationUser _user;
        private readonly ApplicationDbContext _context;
        
        public VendorController(MANCI.UserManager<ApplicationUser> userManager, ApplicationDbContext context) {
            _userManager = userManager;
            _context = context;
        }
        
        public async Task<ApplicationUser> GetUser() {
            return await _userManager.FindByIdAsync(User.Identity.GetUserId());
        }

        public async Task<string> GetUserID() {
            var user = await GetUser();
            return user.Id;
        }

        public async Task<int> GetShopID() {
            _user = await _userManager.FindByIdAsync(User.Identity.GetUserId());
            await _context.Entry(_user).Collection(x => x.Shops).LoadAsync();
            return _user.Shops.First().ShopID;
        }
       
        [HttpPost]
        public async Task<IActionResult> CreateShop([Bind("ShopName")] Shop shop) {
            shop.UserID = await GetUserID();
            try {
                _context.Shops.Add(shop);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Vendor");
            } 
            catch (Exception) {
                // TODO - Redirect to error page
                return RedirectToAction("Index", "Vendor");
            } 
        }
        
        [HttpPost("create")]
        public async Task<IActionResult> UploadNewProduct(IFormFile ProductImage, [Bind("ProductName", "ProductPrice", "ProductQuantity")] Product product)
        {
            product.ShopID = await GetShopID();
            S3Service service = new S3Service();
            // Returns (upload result, errormsg/URL)
            (bool, string) imgResp = await service.UploadToBucket(ProductImage);
            if (imgResp.Item1 == true)
            {
                product.ProductImage = imgResp.Item2;
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Vendor");
            } else
            {
                // TODO - Handle Error (redirect to error page?)
                return RedirectToAction("Index", "Vendor");
            }
        }

        public async void removePlease()
        {
            SNSService service = new SNSService();
            bool it = await service.IsNumberConfirmed("+601111856340");
            System.Diagnostics.Debug.WriteLine($"YOUR NUMBER IS {it}");
        }

        public async Task<IActionResult> EditProduct([Bind("ProductName", "ProductPrice", "ProductQuantity")] Product product, int ProductID) {
            removePlease();
            System.Diagnostics.Debug.WriteLine($"PRODUCT ID {product.ProductImage}");
            Product p = await _context.Products.FindAsync(ProductID);
            p.ProductName = product.ProductName;
            p.ProductPrice = product.ProductPrice;
            p.ProductQuantity = product.ProductQuantity;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Vendor");
        }


        // Page Views
        public IActionResult UploadProductView() {
            return View();
        }

        public IActionResult VerifyPhoneNumberView() {
            return View();
        }

        public string formatNumber(string number) {
            if (!number.Contains("+6"))
                return "+6" + number;
            return number;
        }

        public async Task<IActionResult> AddPhoneNumber(string PhoneNumber) {
            ApplicationUser user = await GetUser();
            SNSService service = new SNSService();
            PhoneNumber = formatNumber(PhoneNumber);
            user.PhoneNumber = PhoneNumber;
            await _context.SaveChangesAsync();
            service.AddToSandbox(PhoneNumber);
            service.AddSubscriberSMS(PhoneNumber);
            return RedirectToAction("Index", "Vendor");
        }

        public async Task<IActionResult> VerifyPhoneNumber(string token) {
            ApplicationUser user = await GetUser();
            System.Diagnostics.Debug.WriteLine($"Token is {token}");
            string number = user.PhoneNumber;
            SNSService service = new SNSService();
            service.ConfirmSandbox(token, number);
            if (await service.IsNumberConfirmed(number)) {
                user.PhoneIsVerified = true;
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Vendor");
            }
            ViewBag.Error = "Invalid/Expired token!";
            return RedirectToAction("VerifyPhoneNumberView", "Vendor");
        }

        public async Task<IActionResult> UpdateOrderStatus(int SOrderID)
        {
            SOrder order = await _context.SOrders.FindAsync(SOrderID);
            order.SOrderStatus = Status.IN_TRANSIT;
            await _context.SaveChangesAsync();
            return RedirectToAction("ViewOrdersView", "Vendor");
        }

        public async Task<IActionResult> ViewOrdersView()
        {
            Shop s = await _context.Shops.FindAsync(await GetShopID());
            await _context.Entry(s).Collection(x => x.SOrders).LoadAsync();
            int orderCount = s.SOrders.Count();
            if (orderCount != 0)
            {
                List<string> ProductNames = new List<string>();
                List<string> UserNames = new List<string>();

                foreach (SOrder o in s.SOrders)
                {
                    ProductNames.Add(_context.Products.FindAsync(o.ProductID).Result.ProductName);
                    ApplicationUser user = await _context.Users.FindAsync(o.UserID);
                    UserNames.Add($"{user.LastName},{user.FirstName}");
                }

                ViewBag.OrderCount = orderCount;
                ViewBag.Orders = s.SOrders.ToList();
                ViewBag.ProductNames = ProductNames;
                ViewBag.UserNames = UserNames;
            }
            return View();
        }

        public async Task<IActionResult> EditProductView(int? ProductID) {
            if (ProductID != null) {
                Product Product = await _context.Products.FindAsync(ProductID);
                if (Product != null) {
                    return View(Product);
                }
            }
            // TODO - Handle Error (redirect to error page?)
            return RedirectToAction("Index", "Vendor");
        }

        public async Task<IActionResult> DeleteProduct(int? ProductID) {
            if (ProductID != null) {
                Product Product = await _context.Products.FindAsync(ProductID);
                if (Product != null) {
                    _context.Products.Remove(Product);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Vendor");
                }
                return RedirectToAction("Index", "Vendor");
            }

            return RedirectToAction("Index", "Vendor");

            // System.Diagnostics.Debug.WriteLine($"Initializing SNS");
            //SNSService SNSService = new SNSService();

            //System.Diagnostics.Debug.WriteLine($"Initialized SNS");

            // SNSService.AddToSandbox(phone3);
            // SNSService.ConfirmSandbox("291627", phone3);

            // SNSService.AddSubscriberSMS(phone1);
            // SNSService.AddSubscriberSMS(phone3);
            //SNSService.SendSMS("+601111856340", "TEST");
            //SNSService.SendSMS("+601120819021", "TEST");

            //return View();
        }

        public async Task<IActionResult> Index() {
            // Get user, load Shop(s) into collection
            _user = await _userManager.FindByIdAsync(User.Identity.GetUserId());
            await _context.Entry(_user).Collection(x => x.Shops).LoadAsync();
            // Add Shop(s) to ViewBag if exist
            var shopCount = _user.Shops.Count;
            ViewBag.hasShop = true;
            if (shopCount==0)
                ViewBag.hasShop = false;
            else {
                var _shop = _user.Shops.First();
                ViewBag.Shop = _shop;
                ViewBag.BucketName = Environment.GetEnvironmentVariable("AWS_IMAGE_BUCKET_NAME");
                await _context.Entry(_shop).Collection(x => x.Products).LoadAsync();
                ViewBag.Products = _shop.Products;
            }
            bool phoneExists = false;
            if (_user.PhoneNumber != null) {
                phoneExists = true;
            }
            System.Diagnostics.Debug.WriteLine($"PHONE IS {phoneExists}");
            bool verified = _user.PhoneIsVerified;
            ViewBag.PhoneExists = phoneExists;
            ViewBag.PhoneIsVerified = verified;
            return View();
        }
    }
}
