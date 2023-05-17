using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HospitalApp.Data;
using HospitalApp.Models;

namespace HospitalApp.Controllers
{
    public class HealthcareSpecialistsController : Controller
    {
        private readonly HospitalAppContext _context;

        public HealthcareSpecialistsController(HospitalAppContext context)
        {
            _context = context;
        }

        // GET: HealthcareSpecialists
        public async Task<IActionResult> Index()
        {
              return _context.HealthcareSpecialist != null ? 
                          View(await _context.HealthcareSpecialist.ToListAsync()) :
                          Problem("Entity set 'HospitalAppContext.HealthcareSpecialist'  is null.");
        }

        // GET: HealthcareSpecialists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.HealthcareSpecialist == null)
            {
                return NotFound();
            }

            var healthcareSpecialist = await _context.HealthcareSpecialist
                .FirstOrDefaultAsync(m => m.HealthcareProviderID == id);
            if (healthcareSpecialist == null)
            {
                return NotFound();
            }

            return View(healthcareSpecialist);
        }

        // GET: HealthcareSpecialists/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: HealthcareSpecialists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HealthcareProviderID,FirstName,LastName,Specialization,Email,Phone,Address")] HealthcareSpecialist healthcareSpecialist)
        {
            if (ModelState.IsValid)
            {
                _context.Add(healthcareSpecialist);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(healthcareSpecialist);
        }

        // GET: HealthcareSpecialists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.HealthcareSpecialist == null)
            {
                return NotFound();
            }

            var healthcareSpecialist = await _context.HealthcareSpecialist.FindAsync(id);
            if (healthcareSpecialist == null)
            {
                return NotFound();
            }
            return View(healthcareSpecialist);
        }

        // POST: HealthcareSpecialists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("HealthcareProviderID,FirstName,LastName,Specialization,Email,Phone,Address")] HealthcareSpecialist healthcareSpecialist)
        {
            if (id != healthcareSpecialist.HealthcareProviderID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(healthcareSpecialist);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HealthcareSpecialistExists(healthcareSpecialist.HealthcareProviderID))
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
            return View(healthcareSpecialist);
        }

        // GET: HealthcareSpecialists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.HealthcareSpecialist == null)
            {
                return NotFound();
            }

            var healthcareSpecialist = await _context.HealthcareSpecialist
                .FirstOrDefaultAsync(m => m.HealthcareProviderID == id);
            if (healthcareSpecialist == null)
            {
                return NotFound();
            }

            return View(healthcareSpecialist);
        }

        // POST: HealthcareSpecialists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.HealthcareSpecialist == null)
            {
                return Problem("Entity set 'HospitalAppContext.HealthcareSpecialist'  is null.");
            }
            var healthcareSpecialist = await _context.HealthcareSpecialist.FindAsync(id);
            if (healthcareSpecialist != null)
            {
                _context.HealthcareSpecialist.Remove(healthcareSpecialist);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HealthcareSpecialistExists(int id)
        {
          return (_context.HealthcareSpecialist?.Any(e => e.HealthcareProviderID == id)).GetValueOrDefault();
        }
    }
}
