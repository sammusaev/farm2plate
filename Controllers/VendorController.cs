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

namespace farm2plate.Controllers
{
    [Authorize(Roles="Vendor")]
    public class VendorController : Controller
    {
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;
        private ApplicationUser _user;
        private readonly ApplicationDbContext _context;
        
     
        public VendorController(Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        
        public async Task<ApplicationUser> GetUser()
        {
            return await _userManager.FindByIdAsync(User.Identity.GetUserId());
        }

        public async Task<string> GetUserID()
        {
            var user = await GetUser();
            return user.Id;
        }
       
        [HttpPost]
        public async Task<IActionResult>createShop([Bind("Name")] Shop shop)
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

        public ActionResult NewProduct()
        {
            return View();
        }
        
        public async Task<IActionResult> Index()
        {
            _user = await _userManager.FindByIdAsync(User.Identity.GetUserId());
            await _context.Entry(_user).Collection(x => x.Shops).LoadAsync();
            System.Diagnostics.Debug.WriteLine($"SHOPS {_user.Shops.Count}");
            var shopCount = _user.Shops.Count;
            if (shopCount==0)
            {
                ViewBag.hasShop = false;
                return View();
            } else
            {
                ViewBag.hasShop = true;
                ViewBag.Shop = _user.Shops.First();
                return View();
            }
           
        }
    }
}
