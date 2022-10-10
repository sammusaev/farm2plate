using farm2plate.Areas.Identity.Data;
using farm2plate.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;

namespace farm2plate.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext context;

        public HomeController
            (ILogger<HomeController> logger, 
            SignInManager<ApplicationUser> signInManager,
            Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager,
            ApplicationDbContext context
            )
        {
            _logger = logger;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.context = context;
        }

        // Consider moving this somewhere else
        // Login essentially calls the same logic
        private async Task<string> getRoleRedirect()
        {
            var user = await userManager.FindByIdAsync(User.Identity.GetUserId());
            var roles = await userManager.GetRolesAsync(user);
            if (roles.Contains("Admin"))
            {
                return "/admin";
            }
            else if (roles.Contains("Vendor")){
                return "/vendor";
            }
            return "/customer";
        }

        public async Task<IActionResult> Index()
        {
            if (signInManager.IsSignedIn(User))
            {
                var user = await userManager.FindByIdAsync(User.Identity.GetUserId());
                await context.Entry(user).Collection(x => x.Shops).LoadAsync();
                return LocalRedirect(await getRoleRedirect());
            }
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
