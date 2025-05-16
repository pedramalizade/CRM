using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CRM.Data;
using CRM.Models;
using Microsoft.AspNetCore.Authorization;
using ReflectionIT.Mvc.Paging;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using CRM.Helpers;

namespace CRM.Controllers
{
    public class CompaniesController : Controller
    {
        private readonly CRMContext _context;

        public CompaniesController(CRMContext context)
        {
            _context = context;
        }

        // GET: Companies
        [Authorize]
        public async Task<IActionResult> Index(string start, string end, string category, int[] selected, int page = 1, string sortExpression = "Id")
        {
            var userClaim = User.FindFirst("user");
            if (userClaim == null || string.IsNullOrEmpty(userClaim.Value))
            {
                return RedirectToAction("Login", "Accounts");
            }

            var user = await _context.User.FirstOrDefaultAsync(m => m.Login == userClaim.Value);
            if (user == null)
            {
                return RedirectToAction("Login", "Accounts");
            }

            var qry = _context.Company.AsNoTracking()
                .Where(p => p.IsDeleted == 0)
                .AsQueryable();

            DateTime? startDate = null;
            DateTime? endDate = null;

            if (!string.IsNullOrEmpty(start) && PersianDateHelper.IsValidPersianDate(start))
            {
                startDate = PersianDateHelper.ToGregorianDate(start);
            }
            if (!string.IsNullOrEmpty(end) && PersianDateHelper.IsValidPersianDate(end))
            {
                endDate = PersianDateHelper.ToGregorianDate(end);
            }

            ViewBag.start = start;
            ViewBag.end = end;
            ViewBag.userId = user.Id;

            if (startDate != null)
            {
                qry = qry.Where(m => m.CreationDate >= startDate);
            }
            if (endDate != null)
            {
                qry = qry.Where(m => m.CreationDate <= endDate);
            }

            ViewBag.selected = selected.Length > 0 ? selected : null;
            if (selected.Length > 0)
            {
                qry = qry.Where(p => selected.Contains(p.BusinessId));
            }

            if (!string.IsNullOrEmpty(sortExpression))
            {
                qry = qry.OrderBy(sortExpression);
            }
            else
            {
                qry = qry.OrderBy(p => p.Id);
            }

            var model = await PagingList.CreateAsync(qry, 3, page, sortExpression, "Id");
            model.RouteValue = new RouteValueDictionary
            {
                { "filter", category },
                { "start", start },
                { "end", end }
            };

            var businesses = _context.Business.ToDictionary(b => b.Id, b => b.Name);
            ViewBag.data = businesses;

            var users = _context.User.ToDictionary(u => u.Id, u => u.Name);
            ViewBag.data2 = users;

            return View(model);
        }

        // GET: Companies/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            var userClaim = User.FindFirst("user");
            if (userClaim == null || string.IsNullOrEmpty(userClaim.Value))
            {
                return RedirectToAction("Login", "Accounts");
            }

            var user = await _context.User.FirstOrDefaultAsync(m => m.Login == userClaim.Value);
            if (user == null)
            {
                return RedirectToAction("Login", "Accounts");
            }

            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Company.FirstOrDefaultAsync(m => m.Id == id);
            if (company == null)
            {
                return NotFound();
            }

            ViewBag.userId = user.Id;
            ViewBag.message = user.Id;

            var businesses = _context.Business.ToDictionary(b => b.Id, b => b.Name);
            ViewBag.data = businesses;

            var users = _context.User.ToDictionary(u => u.Id, u => u.Name);
            ViewBag.data2 = users;

            return View(company);
        }

        // GET: Companies/Create
        [Authorize]
        public async Task<IActionResult> Create()
        {
            var userClaim = User.FindFirst("user");
            if (userClaim == null || string.IsNullOrEmpty(userClaim.Value))
            {
                return RedirectToAction("Login", "Accounts");
            }

            var user = await _context.User.FirstOrDefaultAsync(m => m.Login == userClaim.Value);
            if (user == null)
            {
                return RedirectToAction("Login", "Accounts");
            }

            ViewBag.message = user.Id;
            var businesses = await _context.Business.ToDictionaryAsync(b => b.Id, b => b.Name);
            ViewBag.data = businesses;
            return View();
        }

        // POST: Companies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,NIP,Address,City,UserId,CreationDate")] Company company)
        {
            var userClaim = User.FindFirst("user");
            if (userClaim == null || string.IsNullOrEmpty(userClaim.Value))
            {
                return RedirectToAction("Login", "Accounts");
            }

            var user = await _context.User.FirstOrDefaultAsync(m => m.Login == userClaim.Value);
            if (user == null)
            {
                return RedirectToAction("Login", "Accounts");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    company.IsDeleted = 0;
                    company.CreationDate = DateTime.Now;
                    company.UserId = user.Id; // تنظیم خودکار UserId
                    _context.Add(company);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ViewBag.data = await _context.Business.ToListAsync();
                    ViewBag.message = user.Id;
                    ModelState.AddModelError("", "NIP Is Taken");
                    return View(company);
                }
            }
            //if (ModelState.IsValid)
            //{
            //    try
            //    {
            //        var firstBusiness = await _context.Business.FirstOrDefaultAsync();
            //        if (firstBusiness == null)
            //        {
            //            ModelState.AddModelError("", "هیچ بیزنسی برای انتساب وجود ندارد.");
            //            ViewBag.data = await _context.Business.ToDictionaryAsync(b => b.Id, b => b.Name);
            //            ViewBag.message = user.Id;
            //            return View(company);
            //        }

            //        company.BusinessId = firstBusiness.Id;
            //        company.IsDeleted = 0;
            //        company.CreationDate = DateTime.Now;
            //        company.UserId = user.Id;

            //        _context.Add(company);
            //        await _context.SaveChangesAsync();
            //        return RedirectToAction(nameof(Index));
            //    }
            //    catch (DbUpdateException)
            //    {
            //        ViewBag.data = await _context.Business.ToDictionaryAsync(b => b.Id, b => b.Name);
            //        ViewBag.message = user.Id;
            //        ModelState.AddModelError("", "NIP Is Taken");
            //        return View(company);
            //    }
            //}

            ViewBag.data = await _context.Business.ToDictionaryAsync(b => b.Id, b => b.Name);
            ViewBag.message = user.Id;
            return View(company);
        }

        // GET: Companies/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            var userClaim = User.FindFirst("user");
            if (userClaim == null || string.IsNullOrEmpty(userClaim.Value))
            {
                return RedirectToAction("Login", "Accounts");
            }

            var user = await _context.User.FirstOrDefaultAsync(m => m.Login == userClaim.Value);
            if (user == null)
            {
                return RedirectToAction("Login", "Accounts");
            }

            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Company.FindAsync(id);
            if (company == null || user.Id != company.UserId)
            {
                return NotFound();
            }

            ViewBag.data = await _context.Business.ToDictionaryAsync(b => b.Id, b => b.Name);
            return View(company);
        }

        // POST: Companies/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,NIP,Address,City,UserId,CreationDate,BusinessId")] Company company)
        {
            var userClaim = User.FindFirst("user");
            if (userClaim == null || string.IsNullOrEmpty(userClaim.Value))
            {
                return RedirectToAction("Login", "Accounts");
            }

            var user = await _context.User.FirstOrDefaultAsync(m => m.Login == userClaim.Value);
            if (user == null)
            {
                return RedirectToAction("Login", "Accounts");
            }

            if (id != company.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingCompany = await _context.Company.FindAsync(id);
                    if (existingCompany == null || existingCompany.UserId != user.Id)
                    {
                        return NotFound();
                    }

                    existingCompany.Name = company.Name;
                    existingCompany.NIP = company.NIP;
                    existingCompany.Address = company.Address;
                    existingCompany.City = company.City;
                    // BusinessId, CreationDate و UserId تغییر نمی‌کنند

                    _context.Update(existingCompany);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyExists(company.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                catch (DbUpdateException)
                {
                    ViewBag.data = await _context.Business.ToDictionaryAsync(b => b.Id, b => b.Name);
                    ModelState.AddModelError("", "NIP is taken");
                    return View(company);
                }
            }

            ViewBag.data = await _context.Business.ToDictionaryAsync(b => b.Id, b => b.Name);
            return View(company);
        }

        // GET: Companies/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            var userClaim = User.FindFirst("user");
            if (userClaim == null || string.IsNullOrEmpty(userClaim.Value))
            {
                return RedirectToAction("Login", "Accounts");
            }

            var user = await _context.User.FirstOrDefaultAsync(m => m.Login == userClaim.Value);
            if (user == null)
            {
                return RedirectToAction("Login", "Accounts");
            }

            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Company.FirstOrDefaultAsync(m => m.Id == id);
            if (company == null || user.Id != company.UserId)
            {
                return NotFound();
            }

            ViewBag.userId = user.Id;

            var businesses = _context.Business.ToDictionary(b => b.Id, b => b.Name);
            ViewBag.data = businesses;

            var users = _context.User.ToDictionary(u => u.Id, u => u.Name);
            ViewBag.data2 = users;

            return View(company);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userClaim = User.FindFirst("user");
            if (userClaim == null || string.IsNullOrEmpty(userClaim.Value))
            {
                return RedirectToAction("Login", "Accounts");
            }

            var user = await _context.User.FirstOrDefaultAsync(m => m.Login == userClaim.Value);
            if (user == null)
            {
                return RedirectToAction("Login", "Accounts");
            }

            var company = await _context.Company.FindAsync(id);
            if (company == null || company.UserId != user.Id)
            {
                return NotFound();
            }

            company.IsDeleted = 1;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CompanyExists(int id)
        {
            return _context.Company.Any(e => e.Id == id && e.IsDeleted == 0);
        }
    }
}