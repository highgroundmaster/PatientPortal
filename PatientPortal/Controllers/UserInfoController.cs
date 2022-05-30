using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientPortal.Helpers;
using PatientPortal.Models;

namespace PatientPortal.Controllers
{
    public class UserInfoController : Controller
    {
        private readonly PatientPortalContext _context;

        public UserInfoController(PatientPortalContext context)
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

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> Register([Bind("UserId,UserName,EmailId,Password,PhoneNumber")] Userinfo user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingUser = await _context.Userinfos.FirstOrDefaultAsync(m => m.EmailId == user.EmailId);
                    if (existingUser == null)
                    {
                        user.salt = UserPasswordHelper.GenerateSalt(user.Password.Length);
                        user.Password = UserPasswordHelper.HashPassword(user.Password, user.salt);

                        _context.Add(user);
                        _context.SaveChangesAsync();
                        ModelState.Clear();
                        return RedirectToAction("LogIn", "Login");
                    }
                    ViewBag.ErrorMessage = "Email already registered";
                    
                }
                return View();
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = "Exception : " + e;
                return View();
            }
        }

        public ActionResult Login()
        {
            return View();
        }

            //[ValidateAntiForgeryToken]
            //[HttpPost]
            //public ActionResult Login()
            //{
            //    try
            //    {
            //        using (var context = new CmsDbContext())
            //        {
            //            var getUser = (from s in context.ObjRegisterUser where s.UserName == userName || s.EmailId == userName select s).FirstOrDefault();
            //            if (getUser != null)
            //            {
            //                var hashCode = getUser.VCode;
            //                //Password Hasing Process Call Helper Class Method    
            //                var encodingPasswordString = UserPasswordHelper.HashPassword(user.Password, user.salt);

            //                //Check Login Detail User Name Or Password    
            //                var query = (from s in context.ObjRegisterUser where (s.UserName == userName || s.EmailId == userName) && s.Password.Equals(encodingPasswordString) select s).FirstOrDefault();
            //                if (query != null)
            //                {
            //                    //RedirectToAction("Details/" + id.ToString(), "FullTimeEmployees");    
            //                    //return View("../Admin/Registration"); url not change in browser    
            //                    return RedirectToAction("Index", "Admin");
            //                }
            //                ViewBag.ErrorMessage = "Invallid User Name or Password";
            //                return View();
            //            }
            //            ViewBag.ErrorMessage = "Invallid User Name or Password";
            //            return View();
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        ViewBag.ErrorMessage = " Error!!! contact cms@info.in";
            //        return View();
            //    }
            //}
    }

    
}
