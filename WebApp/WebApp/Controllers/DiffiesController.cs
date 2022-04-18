using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Security.Claims;
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
    [Authorize]
    public class DiffiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DiffiesController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public string GetUserId()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            return claim?.Value ?? "";
        }

        // GET: Diffies
        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            return View(await _context.Diffie.Where(d => d.UserId == userId).ToListAsync());
        }

        // GET: Diffies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = GetUserId();
            
            var diffie = await _context.Diffie
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
            if (diffie == null)
            {
                return NotFound();
            }

            return View(diffie);
        }

        // GET: Diffies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Diffies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PrivateKeyA,PrivateKeyB,ModulusP,BaseG,PublicKey,SharedKey")] Diffie diffie)
        {

            if (!Diffie_Hellman_Static.IsInputOK(diffie.ModulusP, diffie.BaseG, diffie.PrivateKeyA, diffie.PrivateKeyB))
            {
                return View("Diffie_Error");
            }
            
            Diffie_Hellman diffieObjectA = new Diffie_Hellman(diffie.ModulusP, diffie.BaseG, diffie.PrivateKeyA);
            Diffie_Hellman diffieObjectB = new Diffie_Hellman(diffie.ModulusP, diffie.BaseG, diffie.PrivateKeyB);

            diffieObjectA.MakeSharedKey(diffieObjectB.PublicKey);
            diffieObjectB.MakeSharedKey(diffieObjectA.PublicKey);

            if (diffieObjectA.SharedKey == diffieObjectB.SharedKey)
            {
                diffie.PublicKey = diffieObjectA.PublicKey;
                diffie.SharedKey = diffieObjectA.SharedKey;
            }
            else
            {
                return View("Diffie_Error");
            }

            if (ModelState.IsValid)
            {
                var userId = GetUserId();
                diffie.UserId = userId;
                
                _context.Add(diffie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(diffie);
        }

        // GET: Diffies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = GetUserId();
            
            var diffie = await _context.Diffie.Where(d => d.UserId == userId && d.Id == id).FirstOrDefaultAsync();
            if (diffie == null)
            {
                return NotFound();
            }
            return View(diffie);
        }

        // POST: Diffies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PrivateKeyA,PrivateKeyB,ModulusP,BaseG,PublicKey,SharedKey")] Diffie diffie)
        {
            if (id != diffie.Id)
            {
                return NotFound();
            }

            var userId = GetUserId();
            diffie.UserId = userId;
            
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(diffie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DiffieExists(diffie.Id))
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
            return View(diffie);
        }

        // GET: Diffies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = GetUserId();
            
            var diffie = await _context.Diffie
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
            if (diffie == null)
            {
                return NotFound();
            }

            return View(diffie);
        }

        // POST: Diffies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = GetUserId();
            
            var diffie = await _context.Diffie.Where(d => d.UserId == userId && d.Id == id).FirstOrDefaultAsync();
            _context.Diffie.Remove(diffie);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DiffieExists(int id)
        {
            return _context.Diffie.Any(e => e.Id == id);
        }
    }
}
