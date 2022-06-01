using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using PatientPortal.Helpers;
using PatientPortal.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace PatientPortal.Controllers
{
    public class AccountController : Controller
    {
        private readonly PatientPortalContext _context;

        public AccountController(PatientPortalContext context)
        {
            _context = context;
        }

        [Authorize(Roles="Admin")]
        public async Task<IActionResult> Index()
        {
            return _context.Userinfos != null ?
                        View(await _context.Userinfos.ToListAsync()) :
                        Problem("Entity set 'PatientPortalContext.Userinfos'  is null.");
        }

        public IActionResult Register(string ReturnUrl="")
        {
            ViewBag.ReturnUrl = ReturnUrl;
            TempData["ReturnUrl"] = ReturnUrl;
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Register([Bind("UserId,UserName,EmailId,Password,PhoneNumber")] Userinfo user)
        {
            if (TempData.Peek("ReturnUrl") != null)
            {
                string ReturnUrl = TempData["ReturnUrl"] as string;

                if (ModelState.IsValid)
                {
                    // TODO - Changing Admin Authentication
                    var existingUser = await _context.Userinfos.FirstOrDefaultAsync(m => m.UserName == user.UserName);
                    if (existingUser == null)
                    {
                        string role = user.UserName.Contains("jivandeep.org") ? "Admin": "User";
                        // 16 Bytes Salt
                        byte[] salt = UserPasswordHelper.GenerateSalt(16);
                        
                        // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
                        // First 24 Characters is Salt
                        user.Password = Convert.ToBase64String(salt) + UserPasswordHelper.HashPassword(user.Password, salt);
                        //Create the identity for the user  
                        var identity = new ClaimsIdentity(new[] {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.Role, role)
                    }, CookieAuthenticationDefaults.AuthenticationScheme);

                        var principal = new ClaimsPrincipal(identity);

                        var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                        _context.Add(user);
                        await _context.SaveChangesAsync();
                        ModelState.Clear();


                        if (!String.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                        {
                            TempData["UserId"] = user.UserId.ToString();

                            return Redirect(ReturnUrl);
                        }
                        else
                            return RedirectToAction("Index", "Home");
                    }
                    ViewBag.ErrorMessage = "User name already registered";

                }

            }
            return View(user);
        }


        public IActionResult Login(string ReturnUrl = "")
        {
            Console.WriteLine($"Entered Login GET with Return Url - {ReturnUrl}");
            ViewBag.ReturnUrl = ReturnUrl;
            TempData["ReturnUrl"] = ReturnUrl;
            return View();
        }

        
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Login(string userName, string password)
        {
            if (TempData.Peek("ReturnUrl") != null)
            {
                string ReturnUrl = TempData["ReturnUrl"] as string;

                var existingUser = await _context.Userinfos.FirstOrDefaultAsync(m => m.UserName == userName);
                if (existingUser != null)
                {
                    // First 24 Characters is Salt
                    byte[] salt = Convert.FromBase64String(existingUser.Password.Substring(0, 24));

                    string dbPassword = existingUser.Password.Substring(24);
                    
                    // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
                    string userPassword = UserPasswordHelper.HashPassword(password, salt);

                    if (string.Equals(userPassword, dbPassword))
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
                    {
                        TempData["UserId"] = existingUser.UserId.ToString();

                        return Redirect(ReturnUrl);
                    }
                    else
                        return RedirectToAction("Index", "Home");
                }
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