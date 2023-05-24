using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HospitalApp.Data;
using HospitalApp.Models;
using ServiceStack;
using HospitalApp.Services;

namespace HospitalApp.Controllers
{
    public class AppointsController : Controller
    {
        private readonly HospitalAppContext _context;
        private readonly IMailService _service;

        public AppointsController(HospitalAppContext context, IMailService service)
        {
            _context = context;
            _service = service;
        }

        // GET: Appoints
        public async Task<IActionResult> Index()
        {
              return _context.Appoint != null ? 
                          View(await _context.Appoint.ToListAsync()) :
                          Problem("Entity set 'HospitalAppContext.Appoint'  is null.");
        }

        // GET: Appoints/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Appoint == null)
            {
                return NotFound();
            }

            var appoint = await _context.Appoint
                .FirstOrDefaultAsync(m => m.AppointmentID == id);
            if (appoint == null)
            {
                return NotFound();
            }

            return View(appoint);
        }

        // GET: Appoints/Create
        public IActionResult Create()
        {
            ViewData["HealthcareProviderID"] = new SelectList(_context.HealthcareSpecialist, "HealthcareProviderID", "FirstName");
            return View();
        }

        // POST: Appoints/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AppointmentID,PatientID,AppointmentDate,AppointmentTime,AppointmentType,HealthcareProviderID,Notes,Approved")] Appoint appoint)
        {
            if (ModelState.IsValid)
            {
                _context.Add(appoint);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            ViewData["HealthcareProviderID"] = new SelectList(_context.HealthcareSpecialist, "HealthcareProviderID", "FirstName", appoint.HealthcareProviderID);
            return View(appoint);
        }

        // GET: Appoints/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Appoint == null)
            {
                return NotFound();
            }

            var appoint = await _context.Appoint.FindAsync(id);
            if (appoint == null)
            {
                return NotFound();
            }
            return View(appoint);
        }

        // POST: Appoints/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AppointmentID,PatientID,AppointmentDate,AppointmentTime,AppointmentType,HealthcareProviderID,Notes,Approved")] Appoint appoint)
        {
            if (id != appoint.AppointmentID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appoint);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointExists(appoint.AppointmentID))
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
            return View(appoint);
        }

        // GET: Appoints/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Appoint == null)
            {
                return NotFound();
            }

            var appoint = await _context.Appoint
                .FirstOrDefaultAsync(m => m.AppointmentID == id);
            if (appoint == null)
            {
                return NotFound();
            }

            return View(appoint);
        }

        // POST: Appoints/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Appoint == null)
            {
                return Problem("Entity set 'HospitalAppContext.Appoint'  is null.");
            }
            var appoint = await _context.Appoint.FindAsync(id);
            if (appoint != null)
            {
                _context.Appoint.Remove(appoint);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppointExists(int id)
        {
          return (_context.Appoint?.Any(e => e.AppointmentID == id)).GetValueOrDefault();
        }
        public async Task<IActionResult> Approved(int id)
        {
            var appointment = await _context.Appoint.SingleOrDefaultAsync(a => a.AppointmentID == id);
            var patient = await _context.Patient.SingleOrDefaultAsync(b => b.id == appointment.PatientID);
            if (appointment == null)
            {
                return NotFound();
            }

            appointment.Approved = true;
            await _context.SaveChangesAsync();

            var mailContent = new MailContent
            {
                ToEmailAddress = "mpano.akim@gmail.com",
                EmailSubject = "Appointment Approved",
                EmailBody = $"Thank you for booking a appointment  on {appointment.AppointmentDate}   "
            };

            _service.sendEmail(mailContent);

            return Json(new { success = true });

        }
    }
}
