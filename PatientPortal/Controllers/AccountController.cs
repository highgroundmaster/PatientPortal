using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using PatientPortal.Helpers;
using PatientPortal.Models;
using Microsoft.EntityFrameworkCore;

namespace PatientPortal.Controllers
{
    public class AccountController : Controller
    {
        private readonly PatientPortalContext _context;

        public AccountController(PatientPortalContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Register([Bind("UserId,UserName,EmailId,Password,PhoneNumber")] Userinfo user, string ReturnUrl="")
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _context.Userinfos.FirstOrDefaultAsync(m => m.UserName == user.UserName );
                if (existingUser == null)
                {

                    // 16 Bytes Salt
                    byte[] salt = UserPasswordHelper.GenerateSalt(16);
                    Console.WriteLine($"Salt: {Convert.ToBase64String(salt)}");

                    // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
                    // First 24 Characters is Salt
                    user.Password = Convert.ToBase64String(salt) + UserPasswordHelper.HashPassword(user.Password, salt);
                    //Create the identity for the user  
                    var identity = new ClaimsIdentity(new[] {
                        new Claim(ClaimTypes.Name, user.UserName)
                    }, CookieAuthenticationDefaults.AuthenticationScheme);

                    var principal = new ClaimsPrincipal(identity);

                    var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    _context.Add(user);
                    _context.SaveChangesAsync();
                    ModelState.Clear();


                    if (!String.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                        return Redirect(ReturnUrl);
                    else
                        return RedirectToAction(nameof(Index));
                }
                ViewBag.ErrorMessage = "User name already registered";

            }
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Login(string userName, string password, string ReturnUrl = "")
        {
            var existingUser = await _context.Userinfos.FirstOrDefaultAsync(m => m.UserName == userName);
            if (existingUser != null)
            {
                byte[] salt = new byte[16];
                byte[] dbHashedPassword = System.Convert.FromBase64String(existingUser.Password);
                System.Buffer.BlockCopy(dbHashedPassword, 0, salt, 0, salt.Length);
                // First 24 Characters is Salt
                string dbPassword = existingUser.Password.Substring(24);
                Console.WriteLine($"Salt: {Convert.ToBase64String(salt)}");

                // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
                string userPassword = UserPasswordHelper.HashPassword(password, salt);

                if(string.Equals(userPassword, dbPassword))
                {
                    //Create the identity for the user  
                    var identity = new ClaimsIdentity(new[] {
                        new Claim(ClaimTypes.Name, existingUser.UserName)
                    }, CookieAuthenticationDefaults.AuthenticationScheme);

                    var principal = new ClaimsPrincipal(identity);

                    var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                }
                ViewBag.ErrorMessage = "Password Incorrect";


                if (!String.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                    return Redirect(ReturnUrl);
                else
                    return RedirectToAction(nameof(Index));
            }
            ViewBag.ErrorMessage = "User not registered";

            return View();
        }


        [HttpPost]
        public IActionResult Logout()
        {
            var login = HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}