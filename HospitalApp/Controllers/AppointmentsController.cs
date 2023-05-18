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
    public class AppointmentsController : Controller
    {
        private readonly HospitalAppContext _context;
        private readonly IMailService _service;

        public AppointmentsController(HospitalAppContext context, IMailService service)
        {
            _service = service;
            _context = context;
        }

        // GET: Appointments
        [CustomAuthorizeAttribute("Administrator", "HealthcareProfessional")]
        public async Task<IActionResult> Index()
        {
              return _context.Appointment != null ? 
                          View(await _context.Appointment.ToListAsync()) :
                          Problem("Entity set 'HospitalAppContext.Appointment'  is null.");
        }

        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Appointment == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointment
                .FirstOrDefaultAsync(m => m.AppointmentID == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // GET: Appointments/Create
        public IActionResult Create()
        {
            ViewData["PatientID"] = new SelectList(_context.Patient, "id", "fullName");
            ViewData["HealthcareProviderID"] = new SelectList(_context.HealthcareSpecialist, "HealthcareProviderID", "FirstName");
            return View();
        }

        // POST: Appointments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AppointmentID,PatientID,AppointmentDate,AppointmentTime,AppointmentType,HealthcareProviderID,Notes")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PatientID"] = new SelectList(_context.Patient, "id", "fullName", appointment.PatientID);
            ViewData["HealthcareProviderID"] = new SelectList(_context.HealthcareSpecialist, "HealthcareProviderID", "FirstName", appointment.HealthcareProviderID);

            return View(appointment);
        }

        // GET: Appointments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Appointment == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointment.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }
            return View(appointment);
        }

        // POST: Appointments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AppointmentID,PatientID,AppointmentDate,AppointmentTime,AppointmentType,HealthcareProviderID,Notes")] Appointment appointment)
        {
            if (id != appointment.AppointmentID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.AppointmentID))
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
            return View(appointment);
        }

        // GET: Appointments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Appointment == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointment
                .FirstOrDefaultAsync(m => m.AppointmentID == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Appointment == null)
            {
                return Problem("Entity set 'HospitalAppContext.Appointment'  is null.");
            }
            var appointment = await _context.Appointment.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointment.Remove(appointment);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentExists(int id)
        {
          return (_context.Appointment?.Any(e => e.AppointmentID == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> Approved(int id)
        {
            var appointment = await _context.Appointment.SingleOrDefaultAsync(a => a.AppointmentID == id);

            if (appointment == null)
            {
                return NotFound();
            }

            appointment.Approved = true;
            await _context.SaveChangesAsync();

            var mailContent = new MailContent
            {
                ToEmailAddress = "mpanoaki@gmail.com",
                EmailSubject = "Appointment Approved",
                EmailBody = $"Thank you for booking a appointment  on {appointment.AppointmentDate}   "
            };

            _service.sendEmail(mailContent);

            return Json(new { success = true });

        }
    }
}
