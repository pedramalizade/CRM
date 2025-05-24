using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CRM.Data;
using CRM.Models;
using Microsoft.AspNetCore.Authorization;

namespace CRM.Controllers
{
    public class ContactsController : Controller
    {
        private readonly CRMContext _context;

        public ContactsController(CRMContext context)
        {
            _context = context;
        }

        // GET: Contacts
        [Authorize]
        public async Task<IActionResult> Index(string filter)
        {
            // بررسی احراز هویت کاربر
            var userClaim = User.FindFirst("user");
            if (userClaim == null || string.IsNullOrEmpty(userClaim.Value))
            {
                return RedirectToAction("Login", "Accounts");
            }

            var user = await _context.User.FirstOrDefaultAsync(m => m.Login == userClaim.Value && m.IsDeleted == 0);
            if (user == null)
            {
                return RedirectToAction("Login", "Accounts");
            }

            ViewBag.userId = user.Id;

            // دریافت تماس‌ها
            var query = _context.Contact
                .AsNoTracking()
                .Where(c => c.IsDeleted == 0)
                .OrderBy(p => p.Id)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(p => p.Surname.Contains(filter));
            }

            var contacts = await query.ToListAsync();

            // مقداردهی ViewBag.data برای شرکت‌ها
            var companyIds = contacts.Select(c => c.CompanyId).Distinct().ToList();
            ViewBag.data = await _context.Company
                .Where(c => companyIds.Contains(c.Id))
                .ToDictionaryAsync(c => c.Id, c => c.Name ?? "نامشخص");

            // مقداردهی ViewBag.data2 برای کاربران
            var userIds = contacts.Select(c => c.UserId).Distinct().ToList();
            ViewBag.data2 = await _context.User
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => $"{u.Name} {u.Surname}" ?? "نامشخص");

            ViewBag.filter = filter;
            return View(contacts);
        }

        // GET: Contacts/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            var userClaim = User.FindFirst("user");
            if (userClaim == null || string.IsNullOrEmpty(userClaim.Value))
            {
                return RedirectToAction("Login", "Accounts");
            }

            var user = await _context.User.FirstOrDefaultAsync(m => m.Login == userClaim.Value && m.IsDeleted == 0);
            if (user == null)
            {
                return RedirectToAction("Login", "Accounts");
            }

            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contact
                .Where(c => c.IsDeleted == 0)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contact == null)
            {
                return NotFound();
            }

            ViewBag.userId = user.Id;

            // مقداردهی ViewBag.data برای شرکت‌ها
            ViewBag.data = await _context.Company
                .Where(c => c.Id == contact.CompanyId)
                .ToDictionaryAsync(c => c.Id, c => c.Name ?? "نامشخص");

            // مقداردهی ViewBag.data2 برای کاربران
            ViewBag.data2 = await _context.User
                .Where(u => u.Id == contact.UserId)
                .ToDictionaryAsync(u => u.Id, u => $"{u.Name} {u.Surname}" ?? "نامشخص");

            return View(contact);
        }

        // GET: Contacts/Create
        [Authorize]
        public async Task<IActionResult> Create(int? com)
        {
            var userClaim = User.FindFirst("user");
            if (userClaim == null || string.IsNullOrEmpty(userClaim.Value))
            {
                return RedirectToAction("Login", "Accounts");
            }

            var user = await _context.User.FirstOrDefaultAsync(m => m.Login == userClaim.Value && m.IsDeleted == 0);
            if (user == null)
            {
                return RedirectToAction("Login", "Accounts");
            }

            ViewBag.id = com;
            ViewBag.user = user.Id;

            var company = await _context.Company
                .Where(c => c.IsDeleted == 0)
                .FirstOrDefaultAsync(c => c.Id == com);
            if (company != null)
            {
                ViewBag.name = company.Name;
            }

            ViewBag.data = await _context.Company
                .Where(c => c.IsDeleted == 0)
                .ToListAsync();
            return View();
        }

        // POST: Contacts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contact contact)
        {
            var userClaim = User.FindFirst("user");
            if (userClaim == null || string.IsNullOrEmpty(userClaim.Value))
            {
                return RedirectToAction("Login", "Accounts");
            }

            var user = await _context.User.FirstOrDefaultAsync(m => m.Login == userClaim.Value && m.IsDeleted == 0);
            if (user == null)
            {
                return RedirectToAction("Login", "Accounts");
            }

            if (ModelState.IsValid)
            {
                contact.IsDeleted = 0;
                contact.UserId = user.Id;
                _context.Add(contact);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.id = contact.CompanyId;
            ViewBag.user = user.Id;
            var company = await _context.Company
                .Where(c => c.IsDeleted == 0)
                .FirstOrDefaultAsync(c => c.Id == contact.CompanyId);
            if (company != null)
            {
                ViewBag.name = company.Name;
            }

            ViewBag.data = await _context.Company
                .Where(c => c.IsDeleted == 0)
                .ToListAsync();
            return View(contact);
        }

        // GET: Contacts/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            var userClaim = User.FindFirst("user");
            if (userClaim == null || string.IsNullOrEmpty(userClaim.Value))
            {
                return RedirectToAction("Login", "Accounts");
            }

            var user = await _context.User.FirstOrDefaultAsync(m => m.Login == userClaim.Value && m.IsDeleted == 0);
            if (user == null)
            {
                return RedirectToAction("Login", "Accounts");
            }

            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contact
                .Where(c => c.IsDeleted == 0)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (contact == null || user.Id != contact.UserId)
            {
                return NotFound();
            }

            ViewBag.data = await _context.Company
                .Where(c => c.IsDeleted == 0)
                .ToListAsync();
            return View(contact);
        }

        // POST: Contacts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Contact contact)
        {
            var userClaim = User.FindFirst("user");
            if (userClaim == null || string.IsNullOrEmpty(userClaim.Value))
            {
                return RedirectToAction("Login", "Accounts");
            }

            var user = await _context.User.FirstOrDefaultAsync(m => m.Login == userClaim.Value && m.IsDeleted == 0);
            if (user == null)
            {
                return RedirectToAction("Login", "Accounts");
            }

            if (id != contact.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingContact = await _context.Contact
                        .Where(c => c.IsDeleted == 0)
                        .FirstOrDefaultAsync(c => c.Id == id);
                    if (existingContact == null || existingContact.UserId != user.Id)
                    {
                        return NotFound();
                    }

                    existingContact.Name = contact.Name;
                    existingContact.Surname = contact.Surname;
                    existingContact.Phone = contact.Phone;
                    existingContact.Email = contact.Email;
                    existingContact.Position = contact.Position;
                    existingContact.CompanyId = contact.CompanyId;
                    // UserId و IsDeleted تغییر نمی‌کنند

                    _context.Update(existingContact);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactExists(contact.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }

            ViewBag.data = await _context.Company
                .Where(c => c.IsDeleted == 0)
                .ToListAsync();
            return View(contact);
        }

        // GET: Contacts/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            var userClaim = User.FindFirst("user");
            if (userClaim == null || string.IsNullOrEmpty(userClaim.Value))
            {
                return RedirectToAction("Login", "Accounts");
            }

            var user = await _context.User.FirstOrDefaultAsync(m => m.Login == userClaim.Value && m.IsDeleted == 0);
            if (user == null)
            {
                return RedirectToAction("Login", "Accounts");
            }

            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contact
                .Where(c => c.IsDeleted == 0)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contact == null || user.Id != contact.UserId)
            {
                return NotFound();
            }

            ViewBag.userId = user.Id;

            ViewBag.data = await _context.Company
                .Where(c => c.Id == contact.CompanyId)
                .ToDictionaryAsync(c => c.Id, c => c.Name ?? "نامشخص");

            ViewBag.data2 = await _context.User
                .Where(u => u.Id == contact.UserId)
                .ToDictionaryAsync(u => u.Id, u => $"{u.Name} {u.Surname}" ?? "نامشخص");

            return View(contact);
        }

        // POST: Contacts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userClaim = User.FindFirst("user");
            if (userClaim == null || string.IsNullOrEmpty(userClaim.Value))
            {
                return RedirectToAction("Login", "Accounts");
            }

            var user = await _context.User.FirstOrDefaultAsync(m => m.Login == userClaim.Value && m.IsDeleted == 0);
            if (user == null)
            {
                return RedirectToAction("Login", "Accounts");
            }

            var contact = await _context.Contact
                .Where(c => c.IsDeleted == 0)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (contact == null || contact.UserId != user.Id)
            {
                return NotFound();
            }

            contact.IsDeleted = 1;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContactExists(int id)
        {
            return _context.Contact.Any(e => e.Id == id && e.IsDeleted == 0);
        }
    }
}