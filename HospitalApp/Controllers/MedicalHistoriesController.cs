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
    public class MedicalHistoriesController : Controller
    {
        private readonly HospitalAppContext _context;

        public MedicalHistoriesController(HospitalAppContext context)
        {
            _context = context;
        }

        // GET: MedicalHistories
        public async Task<IActionResult> Index()
        {
          //  var hospitalAppContext = _context.MedicalHistory.Include(m => m.Patient);
            return View(await _context.MedicalHistory.ToListAsync());
        }

        // GET: MedicalHistories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.MedicalHistory == null)
            {
                return NotFound();
            }

            var medicalHistory = await _context.MedicalHistory
                .Include(m => m.Patient)
                .FirstOrDefaultAsync(m => m.MedicalHistoryID == id);
            if (medicalHistory == null)
            {
                return NotFound();
            }

            return View(medicalHistory);
        }

        // GET: MedicalHistories/Create
        public IActionResult Create()
        {
            ViewData["PatientID"] = new SelectList(_context.Patient, "id", "fullName");
            return View();
        }

        // POST: MedicalHistories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MedicalHistoryID,PatientID,VisitDate,Diagnosis,Treatment,Medications,Notes")] MedicalHistory medicalHistory)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(medicalHistory);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                    
                }
                Console.WriteLine(medicalHistory);

                ViewData["PatientID"] = new SelectList(_context.Patient, "id", "fullName", medicalHistory.PatientID);
                return View(medicalHistory);
            }
            catch (Exception ex)
            {
                // Handle the exception here
                // You can log the exception or display an error message
                // for debugging purposes
                Console.WriteLine(ex.Message);
                throw; // Rethrow the exception if needed
            }
        }

        // GET: MedicalHistories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.MedicalHistory == null)
            {
                return NotFound();
            }

            var medicalHistory = await _context.MedicalHistory.FindAsync(id);
            if (medicalHistory == null)
            {
                return NotFound();
            }
            ViewData["PatientID"] = new SelectList(_context.Patient, "id", "id", medicalHistory.PatientID);
            return View(medicalHistory);
        }

        // POST: MedicalHistories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MedicalHistoryID,PatientID,VisitDate,Diagnosis,Treatment,Medications,Notes")] MedicalHistory medicalHistory)
        {
            if (id != medicalHistory.MedicalHistoryID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(medicalHistory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicalHistoryExists(medicalHistory.MedicalHistoryID))
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
            ViewData["PatientID"] = new SelectList(_context.Patient, "id", "id", medicalHistory.PatientID);
            return View(medicalHistory);
        }

        // GET: MedicalHistories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.MedicalHistory == null)
            {
                return NotFound();
            }

            var medicalHistory = await _context.MedicalHistory
                .Include(m => m.Patient)
                .FirstOrDefaultAsync(m => m.MedicalHistoryID == id);
            if (medicalHistory == null)
            {
                return NotFound();
            }

            return View(medicalHistory);
        }

        // POST: MedicalHistories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.MedicalHistory == null)
            {
                return Problem("Entity set 'HospitalAppContext.MedicalHistory'  is null.");
            }
            var medicalHistory = await _context.MedicalHistory.FindAsync(id);
            if (medicalHistory != null)
            {
                _context.MedicalHistory.Remove(medicalHistory);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MedicalHistoryExists(int id)
        {
          return (_context.MedicalHistory?.Any(e => e.MedicalHistoryID == id)).GetValueOrDefault();
        }
    }
}
