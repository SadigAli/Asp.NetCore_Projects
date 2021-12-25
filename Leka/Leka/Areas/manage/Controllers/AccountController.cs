using Leka.Models;
using Leka.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Leka.Areas.manage.Controllers
{
    [Area("manage")]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;


        public AccountController(UserManager<AppUser> userManager,
                                 RoleManager<IdentityRole> roleManager,
                                 SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public async  Task<IActionResult> Register()
        {
            AppUser admin = new AppUser
            {
                UserName = "SuperAdmin",
                IsAdmin = true
            };
            await _userManager.CreateAsync(admin, "Admin123");
            await _userManager.AddToRoleAsync(admin, "SuperAdmin");
            return RedirectToAction(nameof(Login));
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM login)
        {
            if (!ModelState.IsValid) return View();

            AppUser user = await _userManager.FindByNameAsync(login.Username);
            if (user == null)
            {
                ModelState.AddModelError("", "Email or Password is incorrect!");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(user, login.Password, true, false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Email or Password is incorrect!");
                return View();
            }
            return RedirectToAction("index", "dashboard", new { Area="manage" });
        }

        public async Task CreateRole()
        {
            if (!await _roleManager.RoleExistsAsync("SuperAdmin"))
                await _roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
        }
    }
}
