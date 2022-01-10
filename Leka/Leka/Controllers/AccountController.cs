using Leka.DAL;
using Leka.Models;
using Leka.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Leka.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager,
                                 RoleManager<IdentityRole> roleManager,
                                 SignInManager<AppUser> signInManager,
                                 AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _context = context;
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM register)
        {
            if (!ModelState.IsValid) return View();

            AppUser user = await _userManager.FindByNameAsync(register.Username);
            if (user != null)
            {
                ModelState.AddModelError("Username", "This user is already existed!");
                return View();
            }

            user = new AppUser
            {
                Email = register.Email,
                UserName = register.Username,
                Fullname = register.Fullname,
                IsAdmin = false
            };

            var result = await _userManager.CreateAsync(user, register.Password);

            if (!result.Succeeded)
            {
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                    return View();
                }
            }

            await _userManager.AddToRoleAsync(user, "Member");
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
            return RedirectToAction("index", "home");
        }

        [Authorize]
        public IActionResult ResetPassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM reset)
        {
            if (!ModelState.IsValid) return View();
            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (!string.IsNullOrEmpty(reset.NewPassword) && !string.IsNullOrEmpty(reset.ConfirmNewPassword))
            {
                var result = await _userManager.ChangePasswordAsync(user, reset.OldPassword, reset.NewPassword);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                        return View();
                    }
                }
            }
            if (user.NormalizedEmail != reset.Email.ToUpper() && _userManager.Users.Any(x => x.NormalizedEmail == reset.Email.ToUpper()))
            {
                ModelState.AddModelError("", "This email is already taken!");
                return View();
            }
            user.Email = reset.Email;
            user.UserName = reset.Username;
            user.Fullname = reset.Fullname;
            await _userManager.UpdateAsync(user);
            await _signInManager.SignOutAsync();

            return RedirectToAction("login");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Redirect(HttpContext.Request.Headers["Referer"].ToString());
        }

    }
}
