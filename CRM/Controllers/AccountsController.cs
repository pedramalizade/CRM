using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CRM.Data;
using CRM.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using CRM.Helpers;

namespace CRM.Controllers
{
    public class AccountsController : Controller
    {
        private readonly CRMContext _context;

        public AccountsController(CRMContext context)
        {
            _context = context;
        }

        const string UserName = "_Name";
        const string SessionAge = "_Age";

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Details()
        {
            var userClaim = User.FindFirst("user");
            if (userClaim == null || string.IsNullOrEmpty(userClaim.Value))
            {
                return RedirectToAction("Login");
            }

            var user = await _context.User.FirstOrDefaultAsync(m => m.Login == userClaim.Value);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        private async Task<bool> ValidateLoginAsync(string userName, string password)
        {
            var user = await _context.User.FirstOrDefaultAsync(m => m.Login == userName);
            if (user == null || user.IsDeleted != 0)
            {
                return false;
            }

            string hashedPassword = HashPassword(password);
            return hashedPassword == user.Password;
        }

        private async Task<string> ReturnRole(string userName)
        {
            var user = await _context.User.FirstOrDefaultAsync(m => m.Login == userName);
            if (user == null)
            {
                return string.Empty;
            }
            var role = await _context.Role.FindAsync(user.RoleId);
            return role?.Name ?? string.Empty;
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                if (await ValidateLoginAsync(model.UserName, model.Password))
                {
                    HttpContext.Session.SetString(UserName, model.UserName);
                    string role = await ReturnRole(model.UserName);
                    var claims = new List<Claim>
                    {
                        new Claim("user", model.UserName),
                        new Claim("role", role),
                        new Claim(ClaimTypes.Name, model.UserName),
                        new Claim(ClaimTypes.NameIdentifier, model.UserName)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    return RedirectToAction("Index", "Companies");
                }
                else
                {
                    ModelState.AddModelError("", "نام کاربری یا رمز عبور نامعتبر است");
                    return View(model);
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User user, string DateOfBirth)
        {
            try
            {
                var existingUser = await _context.User.FirstOrDefaultAsync(m => m.Login == user.Login);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "نام کاربری قبلاً استفاده شده است");
                    return View(user);
                }

                if (!PersianDateHelper.IsValidPersianDate(DateOfBirth))
                {
                    ModelState.AddModelError("DateOfBirth", "تاریخ شمسی نامعتبر است");
                    return View(user);
                }

                var gregorianDate = PersianDateHelper.ToGregorianDate(DateOfBirth);
                if (gregorianDate == null)
                {
                    ModelState.AddModelError("DateOfBirth", "تبدیل تاریخ شمسی به میلادی ممکن نیست");
                    return View(user);
                }

                user.DateOfBirth = gregorianDate.Value;

                if (ModelState.IsValid)
                {
                    user.Password = HashPassword(user.Password);
                    user.IsDeleted = 0;
                    _context.Add(user);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Login");
                }
                return View(user);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "خطایی در فرآیند ثبت‌نام رخ داد");
                return View(user);
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit()
        {
            var userClaim = User.FindFirst("user");
            if (userClaim == null || string.IsNullOrEmpty(userClaim.Value))
            {
                return RedirectToAction("Login");
            }

            var user = await _context.User.FirstOrDefaultAsync(m => m.Login == userClaim.Value);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User user, string DateOfBirth)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            var adminCount = await _context.User.CountAsync(m => m.RoleId == 1 && m.IsDeleted == 0);
            ViewBag.Message = null;

            try
            {
                int loginStatus = await LoginUnChangedAsync(id, user.Login);
                if (loginStatus < 3)
                {
                    if (!string.IsNullOrEmpty(DateOfBirth))
                    {
                        if (!PersianDateHelper.IsValidPersianDate(DateOfBirth))
                        {
                            ModelState.AddModelError("DateOfBirth", "تاریخ شمسی نامعتبر است");
                            return View(user);
                        }

                        var gregorianDate = PersianDateHelper.ToGregorianDate(DateOfBirth);
                        if (gregorianDate == null)
                        {
                            ModelState.AddModelError("DateOfBirth", "تبدیل تاریخ شمسی به میلادی ممکن نیست");
                            return View(user);
                        }
                        user.DateOfBirth = gregorianDate.Value;
                    }

                    if (ModelState.IsValid)
                    {
                        if (adminCount > 2 || user.RoleId == 1)
                        {
                            if (!IsMD5(user.Password))
                            {
                                user.Password = HashPassword(user.Password);
                            }
                            _context.Update(user);
                            await _context.SaveChangesAsync();

                            if (loginStatus == 1 || user.RoleId.ToString() != User.FindFirst("role")?.Value)
                            {
                                await HttpContext.SignOutAsync();
                                string role = await ReturnRole(user.Login);
                                var claims = new List<Claim>
                                {
                                    new Claim("user", user.Login),
                                    new Claim("role", role),
                                    new Claim(ClaimTypes.Name, user.Login),
                                    new Claim(ClaimTypes.NameIdentifier, user.Login)
                                };

                                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                                await HttpContext.SignInAsync(
                                    CookieAuthenticationDefaults.AuthenticationScheme,
                                    new ClaimsPrincipal(claimsIdentity));
                            }
                            return RedirectToAction("Details");
                        }
                        else
                        {
                            ViewBag.Message = "نمی‌توانید مدیران بیشتری حذف کنید! حداقل باید ۲ مدیر وجود داشته باشد!";
                            return View(user);
                        }
                    }
                    return View(user);
                }
                else
                {
                    ModelState.AddModelError("", "نام کاربری قبلاً استفاده شده است");
                    return View(user);
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "داده‌های نامعتبر");
                return View(user);
            }
        }

        public IActionResult AccessDenied(string returnUrl = null)
        {
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/");
        }

        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User.FirstOrDefaultAsync(m => m.Id == id && m.IsDeleted == 0);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null || user.IsDeleted != 0)
            {
                return NotFound();
            }

            var adminCount = await _context.User.CountAsync(m => m.RoleId == 1 && m.IsDeleted == 0);
            if (adminCount > 2 || user.RoleId != 1)
            {
                user.IsDeleted = 1;
                await _context.SaveChangesAsync();
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Redirect("/");
            }
            else
            {
                ViewBag.Message = "نمی‌توانید مدیران بیشتری حذف کنید! حداقل باید ۲ مدیر وجود داشته باشد!";
                return View(user);
            }
        }

        public string HashPassword(string password)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashedPassword = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashedPassword.Length; i++)
                {
                    sb.Append(hashedPassword[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

        public static bool IsMD5(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }
            return Regex.IsMatch(input, "^[0-9a-fA-F]{32}$", RegexOptions.Compiled);
        }

        public async Task<int> LoginUnChangedAsync(int id, string login)
        {
            var currentUserClaim = User.FindFirst("user");
            if (currentUserClaim != null && login == currentUserClaim.Value)
            {
                return 1; // Login بدون تغییر
            }

            var user = await _context.User.FirstOrDefaultAsync(m => m.Login == login && m.IsDeleted == 0);
            if (user == null)
            {
                return 2; // Login جدید و آزاد
            }
            return 3; // Login قبلاً استفاده شده
        }
    }
}