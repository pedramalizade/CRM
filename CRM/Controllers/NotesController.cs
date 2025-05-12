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

namespace CRM.Controllers
{
    public class NotesController : Controller
    {
        private readonly CRMContext _context;

        public NotesController(CRMContext context)
        {
            _context = context;
        }

        // GET: Notes
        [Authorize]
        public async Task<IActionResult> Index()
        {
            // چک کردن کاربر لاگین‌شده
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

            ViewBag.userId = user.Id;

            // تنظیم ViewBag.data برای Company
            var companies = _context.Company
                .Where(c => c.IsDeleted == 0)
                .ToDictionary(c => c.Id, c => c.Name);
            ViewBag.data = companies;

            // تنظیم ViewBag.data2 برای User
            var users = _context.User
                .Where(u => u.IsDeleted == 0)
                .ToDictionary(u => u.Id, u => u.Login);
            ViewBag.data2 = users;

            var notes = await _context.Note
                .Where(n => n.IsDeleted == 0)
                .OrderBy(n => n.Id)
                .ToListAsync();

            return View(notes);
        }

        // GET: Notes/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            // چک کردن کاربر لاگین‌شده
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

            var note = await _context.Note
                .Where(n => n.IsDeleted == 0)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (note == null)
            {
                return NotFound();
            }

            ViewBag.userId = user.Id;

            // تنظیم ViewBag.data برای Company
            var companies = _context.Company
                .Where(c => c.IsDeleted == 0)
                .ToDictionary(c => c.Id, c => c.Name);
            ViewBag.data = companies;

            // تنظیم ViewBag.data2 برای User
            var users = _context.User
                .Where(u => u.IsDeleted == 0)
                .ToDictionary(u => u.Id, u => u.Login);
            ViewBag.data2 = users;

            return View(note);
        }

        // GET: Notes/Create
        [Authorize]
        public async Task<IActionResult> Create(int? com)
        {
            // چک کردن کاربر لاگین‌شده
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

        // POST: Notes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Note note)
        {
            // چک کردن کاربر لاگین‌شده
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
                note.IsDeleted = 0;
                note.UserId = user.Id; // تنظیم خودکار UserId
                _context.Add(note);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.id = note.CompanyId;
            ViewBag.user = user.Id;
            var company = await _context.Company
                .Where(c => c.IsDeleted == 0)
                .FirstOrDefaultAsync(c => c.Id == note.CompanyId);
            if (company != null)
            {
                ViewBag.name = company.Name;
            }

            ViewBag.data = await _context.Company
                .Where(c => c.IsDeleted == 0)
                .ToListAsync();
            return View(note);
        }

        // GET: Notes/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            // چک کردن کاربر لاگین‌شده
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

            var note = await _context.Note
                .Where(n => n.IsDeleted == 0)
                .FirstOrDefaultAsync(n => n.Id == id);
            if (note == null || user.Id != note.UserId)
            {
                return NotFound();
            }

            ViewBag.data = await _context.Company
                .Where(c => c.IsDeleted == 0)
                .ToListAsync();
            return View(note);
        }

        // POST: Notes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Note note)
        {
            // چک کردن کاربر لاگین‌شده
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

            if (id != note.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingNote = await _context.Note
                        .Where(n => n.IsDeleted == 0)
                        .FirstOrDefaultAsync(n => n.Id == id);
                    if (existingNote == null || existingNote.UserId != user.Id)
                    {
                        return NotFound();
                    }

                    //existingNote.Title = note.Title;
                    existingNote.Content = note.Content;
                    existingNote.CompanyId = note.CompanyId;
                    // UserId و IsDeleted تغییر نمی‌کنن

                    _context.Update(existingNote);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NoteExists(note.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }

            ViewBag.data = await _context.Company
                .Where(c => c.IsDeleted == 0)
                .ToListAsync();
            return View(note);
        }

        // GET: Notes/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            // چک کردن کاربر لاگین‌شده
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

            var note = await _context.Note
                .Where(n => n.IsDeleted == 0)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (note == null || user.Id != note.UserId)
            {
                return NotFound();
            }

            ViewBag.userId = user.Id;

            // تنظیم ViewBag.data برای Company
            var companies = _context.Company
                .Where(c => c.IsDeleted == 0)
                .ToDictionary(c => c.Id, c => c.Name);
            ViewBag.data = companies;

            // تنظیم ViewBag.data2 برای User
            var users = _context.User
                .Where(u => u.IsDeleted == 0)
                .ToDictionary(u => u.Id, u => u.Login);
            ViewBag.data2 = users;

            return View(note);
        }

        // POST: Notes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // چک کردن کاربر لاگین‌شده
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

            var note = await _context.Note
                .Where(n => n.IsDeleted == 0)
                .FirstOrDefaultAsync(n => n.Id == id);
            if (note == null || note.UserId != user.Id)
            {
                return NotFound();
            }

            note.IsDeleted = 1;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NoteExists(int id)
        {
            return _context.Note.Any(e => e.Id == id && e.IsDeleted == 0);
        }
    }
}
