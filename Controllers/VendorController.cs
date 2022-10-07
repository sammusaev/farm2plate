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

namespace farm2plate.Controllers
{
    [Authorize(Roles="Vendor")]
    public class VendorController : Controller
    {
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;
        private ApplicationUser _user;
        private readonly ApplicationDbContext _context;
        
     
        public VendorController(Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager, ApplicationDbContext context) {
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
        public async Task<IActionResult>createShop([Bind("ShopName")] Shop shop)
        {
            shop.UserID = await GetUserID();
            try {
                _context.Shops.Add(shop);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Vendor");
            } 
            catch (Exception ex) {
                return RedirectToAction("Index", "Vendor");
            } 
        }
        
        [HttpPost("create")]
        public async Task<IActionResult> UploadNewProduct(IFormFile ProductImage, [Bind("ProductName", "ProductPrice", "unitsLeft")] Product product)
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


        // Page Views
        public IActionResult NewProduct() {
            return View();
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
            else
                ViewBag.Shop = _user.Shops.First();
            return View();
        }
    }
}
