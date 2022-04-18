using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Crypto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Domain;
using Microsoft.AspNetCore.Authorization;
using WebApp.Data;

namespace WebApp.Controllers
{
    // This makes it so that the user is logged in to see this page
    [Authorize]
    public class CaesarsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CaesarsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public string GetUserId()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            return claim?.Value ?? "";
        }
        
        // GET: Caesars
        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            return View(await _context.Caesars.Where(c => c.UserId == userId).ToListAsync());
        }

        // GET: Caesars/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = GetUserId();
            
            var caesar = await _context.Caesars
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
            if (caesar == null)
            {
                return NotFound();
            }

            return View(caesar);
        }

        // GET: Caesars/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Caesars/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Key,Plaintext,Ciphertext")] Caesar caesar)
        {
            caesar.Ciphertext = CaesarCipher.EncryptString(caesar.Plaintext, caesar.Key, Encoding.Default);
            
            if (ModelState.IsValid)
            {
                // add UserId foreign key to caesar input
                caesar.UserId = GetUserId();
                
                _context.Add(caesar);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(caesar);
        }

        // GET: Caesars/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = GetUserId();

            var caesar = await _context.Caesars.Where(c => c.UserId == userId && c.Id == id).FirstOrDefaultAsync();
            if (caesar == null)
            {
                return NotFound();
            }
            return View(caesar);
        }

        // POST: Caesars/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Key,Plaintext,Ciphertext")] Caesar caesar)
        {
            if (id != caesar.Id)
            {
                return NotFound();
            }

            var userId = GetUserId();
            caesar.UserId = userId;
            
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(caesar);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CaesarExists(caesar.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(caesar);
        }

        // GET: Caesars/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = GetUserId();
            
            var caesar = await _context.Caesars
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
            if (caesar == null)
            {
                return NotFound();
            }

            return View(caesar);
        }

        // POST: Caesars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = GetUserId();
            
            var caesar = await _context.Caesars.Where(c => c.UserId == userId && c.Id == id).FirstOrDefaultAsync();
            _context.Caesars.Remove(caesar);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CaesarExists(int id)
        {
            return _context.Caesars.Any(e => e.Id == id);
        }
    }
}
