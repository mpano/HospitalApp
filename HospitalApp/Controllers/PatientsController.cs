﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HospitalApp.Data;
using HospitalApp.Models;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace HospitalApp.Controllers
{
    public class PatientsController : Controller
    {
        private readonly HospitalAppContext _context;

        public PatientsController(HospitalAppContext context)
        {
            _context = context;
        }

        // GET: Patients
        /*        public async Task<IActionResult> Index()
                {
                    var patients = await _context.Patient
                        .Where(p => p != null) // Perform null check on fullName property
                        .ToListAsync();

                    return View(patients);
                }*/
        [CustomAuthorizeAttribute("Administrator", "HealthcareProfessional", "Patient")]
        public async Task<IActionResult> Index()
        {
            string userRole = HttpContext.Session.GetString("Role");

            if (userRole == Role.Patient.ToString())
            {
                string userEmail = HttpContext.Session.GetString("Username");
                var patient = _context.Patient.Where(p => p.Email == userEmail && p != null).ToList();

                if (patient.Count > 0)
                {
                    var patientId = patient.First().id;
                    HttpContext.Session.SetString("Patient", patientId.ToString());
                }

                return View(patient);
            }
            else
            {
                var patients = await _context.Patient.Where(p => p != null).ToListAsync();
                return View(patients);
            }
        }

        // GET: Patients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Patient == null)
            {
                return NotFound();
            }

            var patient = await _context.Patient
                .FirstOrDefaultAsync(m => m.id == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // GET: Patients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Patients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,fullName,DateOfBirth,Gender,Address,Phone,Email,InsuranceProvider,InsurancePolicyNumber")] Patient patient)
        {
            if (ModelState.IsValid)
            {
                _context.Add(patient);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(patient);
        }

        // GET: Patients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Patient == null)
            {
                return NotFound();
            }

            var patient = await _context.Patient.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }
            return View(patient);
        }

        // POST: Patients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,fullName,DateOfBirth,Gender,Address,Phone,Email,InsuranceProvider,InsurancePolicyNumber")] Patient patient)
        {
            if (id != patient.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patient);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientExists(patient.id))
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
            return View(patient);
        }

        // GET: Patients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Patient == null)
            {
                return NotFound();
            }

            var patient = await _context.Patient
                .FirstOrDefaultAsync(m => m.id == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // POST: Patients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Patient == null)
            {
                return Problem("Entity set 'HospitalAppContext.Patient'  is null.");
            }
            var patient = await _context.Patient.FindAsync(id);
            if (patient != null)
            {
                _context.Patient.Remove(patient);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PatientExists(int id)
        {
          return (_context.Patient?.Any(e => e.id == id)).GetValueOrDefault();
        }
        private byte[] GeneratePdf(List<Patient> medicalHistories)
        {
            // Create a new PDF document
            var document = new Document();
            var memoryStream = new MemoryStream();
            var writer = PdfWriter.GetInstance(document, memoryStream);
            document.Open();

            // Add content to the PDF document
            var paragraph = new Paragraph("Patient List");
            document.Add(paragraph);

            // Add the medical histories data to the PDF document
            foreach (var medicalHistory in medicalHistories)
            {
                var historyParagraph = new Paragraph($"Patient Id:  {medicalHistory.id}, Full name: {medicalHistory.fullName}, Email: {medicalHistory.Email}, Ensurance: {medicalHistory.InsuranceProvider}");
                document.Add(historyParagraph);
            }

            document.Close();

            // Return the PDF document as bytes
            return memoryStream.ToArray();
        }
        public async Task<IActionResult> DownloadPdf()
        {
            string userRole = HttpContext.Session.GetString("Role");

            if (userRole == Role.Patient.ToString())
            {
                string userEmail = HttpContext.Session.GetString("Username");
                var patient = _context.Patient.Where(p => p.Email == userEmail && p != null).ToList();

                if (patient.Count > 0)
                {
                    var patientId = patient.First().id;
                    HttpContext.Session.SetString("Patient", patientId.ToString());
                }

                return View(patient);
            }
            else
            {
                var patients = await _context.Patient.Where(p => p != null).ToListAsync();
                var pdfBytes = GeneratePdf(patients);
                return File(pdfBytes, "application/pdf", "patientlist.pdf");
                
            }
            
        }
    }
}
