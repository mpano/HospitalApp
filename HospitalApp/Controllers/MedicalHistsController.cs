using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HospitalApp.Data;
using HospitalApp.Models;
using Org.BouncyCastle.Utilities.Collections;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace HospitalApp.Controllers
{
    public class MedicalHistsController : Controller
    {
        private readonly HospitalAppContext _context;

        public MedicalHistsController(HospitalAppContext context)
        {
            _context = context;
        }

        // GET: MedicalHists
        /*      public async Task<IActionResult> Index()
              {
                    return _context.MedicalHist != null ? 
                                View(await _context.MedicalHist.ToListAsync()) :
                                Problem("Entity set 'HospitalAppContext.MedicalHist'  is null.");
              }*/
        [CustomAuthorizeAttribute("Administrator", "HealthcareProfessional", "Patient")]
        public async Task<IActionResult> Index()
        {
            string userRole = HttpContext.Session.GetString("Role");

            if (userRole == Role.Patient.ToString())
            {
                string userEmail = HttpContext.Session.GetString("Username");
                var medicalHistories = await _context.MedicalHist.Where(mh => mh.PatientEmail == userEmail).ToListAsync();
                return View(medicalHistories);
            }
            else
            {
               
                var medicalHistories = await _context.MedicalHist.ToListAsync(); 
                return View(medicalHistories);
            }
        }
        public async Task<IActionResult> DownloadPdf()
        {
            string userRole = HttpContext.Session.GetString("Role");

            if (userRole == Role.Patient.ToString())
            {
                string userEmail = HttpContext.Session.GetString("Username");
                var medicalHistories = await _context.MedicalHist.Where(mh => mh.PatientEmail == userEmail).ToListAsync();
                return View(medicalHistories);
            }
            else
            {
               
                var medicalHistories = await _context.MedicalHist.ToListAsync();
                var pdfBytes = GeneratePdf(medicalHistories);
                return File(pdfBytes, "application/pdf", "MedicalHistories.pdf");
                
            }
            
        }

        // GET: MedicalHists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.MedicalHist == null)
            {
                return NotFound();
            }

            var medicalHist = await _context.MedicalHist
                .FirstOrDefaultAsync(m => m.Id == id);
            if (medicalHist == null)
            {
                return NotFound();
            }

            return View(medicalHist);
        }

        // GET: MedicalHists/Create
        public IActionResult Create()
        {
            ViewData["PatientID"] = new SelectList(_context.Patient, "id", "fullName");
            return View();
        }

        // POST: MedicalHists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PatientID,VisitDate,Diagnosis,Treatment,Medications,Notes,PatientEmail")] MedicalHist medicalHist)
        {
            if (ModelState.IsValid)
            {
                // Retrieve the selected patient
                var selectedPatient = await _context.Patient.FindAsync(medicalHist.PatientID);

                if (selectedPatient != null)
                {
                    // Assign the patient's email to the PatientEmail property
                    medicalHist.PatientEmail = selectedPatient.Email;

                    // Add the history object to the context and save changes
                    _context.Add(medicalHist);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
            }

            ViewData["PatientID"] = new SelectList(_context.Patient, "id", "fullName", medicalHist.PatientID);
            return View(medicalHist);
        }

        // GET: MedicalHists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.MedicalHist == null)
            {
                return NotFound();
            }

            var medicalHist = await _context.MedicalHist.FindAsync(id);
            if (medicalHist == null)
            {
                return NotFound();
            }
            return View(medicalHist);
        }

        // POST: MedicalHists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PatientID,VisitDate,Diagnosis,Treatment,Medications,Notes,PatientEmail")] MedicalHist medicalHist)
        {
            if (id != medicalHist.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(medicalHist);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicalHistExists(medicalHist.Id))
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
            return View(medicalHist);
        }

        // GET: MedicalHists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.MedicalHist == null)
            {
                return NotFound();
            }

            var medicalHist = await _context.MedicalHist
                .FirstOrDefaultAsync(m => m.Id == id);
            if (medicalHist == null)
            {
                return NotFound();
            }

            return View(medicalHist);
        }

        // POST: MedicalHists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.MedicalHist == null)
            {
                return Problem("Entity set 'HospitalAppContext.MedicalHist'  is null.");
            }
            var medicalHist = await _context.MedicalHist.FindAsync(id);
            if (medicalHist != null)
            {
                _context.MedicalHist.Remove(medicalHist);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MedicalHistExists(int id)
        {
          return (_context.MedicalHist?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        private byte[] GeneratePdf(List<MedicalHist> medicalHistories)
        {
            // Create a new PDF document
            var document = new Document();
            var memoryStream = new MemoryStream();
            var writer = PdfWriter.GetInstance(document, memoryStream);
            document.Open();

            // Add content to the PDF document
            var paragraph = new Paragraph("Medical Histories");
            document.Add(paragraph);

            // Add the medical histories data to the PDF document
            foreach (var medicalHistory in medicalHistories)
            {
                var historyParagraph = new Paragraph($"Patient Id:  {medicalHistory.PatientID}, Diagnosis: {medicalHistory.Diagnosis},Visit Date: {medicalHistory.VisitDate}, Diagnosis: {medicalHistory.Diagnosis}");
                document.Add(historyParagraph);
            }

            document.Close();

            // Return the PDF document as bytes
            return memoryStream.ToArray();
        }

    }
}
