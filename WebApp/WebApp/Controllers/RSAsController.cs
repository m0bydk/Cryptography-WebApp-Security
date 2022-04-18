using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Crypto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using RSA = Domain.RSA;

namespace WebApp.Controllers
{
    [Authorize]
    public class RSAsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RSAsController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public string GetUserId()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            return claim?.Value ?? "";
        }

        // GET: RSAs
        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            return View(await _context.RSA.Where(r => r.UserId == userId).ToListAsync());
        }

        // GET: RSAs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = GetUserId();
            
            var rSA = await _context.RSA
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
            if (rSA == null)
            {
                return NotFound();
            }

            return View(rSA);
        }

        // GET: RSAs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: RSAs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,P,Q,N,E,M,D,Plaintext,Ciphertext")] RSA rsa)
        {

            if (RSA_static.CheckPrimes(rsa.P, rsa.Q))
            {
                Crypto.RSA chuck = new Crypto.RSA(rsa.P, rsa.Q);

                rsa.N = chuck.n;
                rsa.E = chuck.e;
                rsa.M = chuck.m;
                rsa.D = chuck.d;
                rsa.Ciphertext = chuck.EncryptString(rsa.Plaintext);
            }
            else
            {
                return View("RSA_Error");
            }
            
            
            if (ModelState.IsValid)
            {
                var userId = GetUserId();
                rsa.UserId = userId;
                
                _context.Add(rsa);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(rsa);
        }

        // GET: RSAs/Decrypt
        public IActionResult Decrypt()
        {
            return View();
        }
        
        // POST: RSAs/Decrypt
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Decrypt([Bind("Id,P,Q,N,E,M,D,Plaintext,Ciphertext")] RSA rsa)
        {

            if (rsa.N > rsa.D)
            {
                rsa.P = 0;
                rsa.Q = 0;
                rsa.E = 0;
                rsa.M = 0;

                rsa.Plaintext = RSA_static.DecryptString(rsa.D, rsa.N, rsa.Ciphertext);
            }
            else
            {
                return View("RSA_Error");
            }
            
            
            if (ModelState.IsValid)
            {
                var userId = GetUserId();
                rsa.UserId = userId;
                
                _context.Add(rsa);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(rsa);
        }

        
        
        // GET: RSAs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = GetUserId();
            
            var rSA = await _context.RSA.Where(r => r.UserId == userId && r.Id == id).FirstOrDefaultAsync();
            if (rSA == null)
            {
                return NotFound();
            }
            return View(rSA);
        }

        // POST: RSAs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,P,Q,N,E,M,D,Plaintext,Ciphertext")] RSA rSA)
        {
            if (id != rSA.Id)
            {
                return NotFound();
            }
            
            var userId = GetUserId();
            rSA.UserId = userId;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rSA);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RSAExists(rSA.Id))
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
            return View(rSA);
        }

        // GET: RSAs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = GetUserId();
            
            var rSA = await _context.RSA
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
            if (rSA == null)
            {
                return NotFound();
            }

            return View(rSA);
        }

        // POST: RSAs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = GetUserId();
            
            var rSA = await _context.RSA.Where(r => r.UserId == userId && r.Id == id).FirstOrDefaultAsync();
            _context.RSA.Remove(rSA);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RSAExists(int id)
        {
            return _context.RSA.Any(e => e.Id == id);
        }
    }
}
