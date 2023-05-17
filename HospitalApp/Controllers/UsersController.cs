using HospitalApp.Data;
using HospitalApp.Models;
using HospitalApp.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace HospitalApp.Controllers
{
    public class UsersController : Controller
    {
        private readonly HospitalAppContext _context;

        public UsersController(HospitalAppContext context)
        {
            _context = context;
        }

        // GET: User/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: User/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(PatientDto user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == user.Username);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "Username already exists.");
                    return View(user);
                }

                var patient = new Patient
                {
                    fullName = user.fullName,
                    Phone = user.Phone,
                    Email = user.Email,
                    Address = ".",
                    Gender = ".",
                    InsuranceProvider = "RAM",
                    InsurancePolicyNumber = "0",
                    
                };

                _context.Patient.Add(patient);
                await _context.SaveChangesAsync();

                var tbuser = new User
                {
                    Username = user.Username,
                    Password = HashPassword(user.Password), // Encrypt the password
                };

                _context.Users.Add(tbuser);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Login));
            }

            return View(user);
        }

        // GET: User/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: User/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(User user)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == user.Username);

            if (existingUser == null || !VerifyPassword(user.Password, existingUser.Password))
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return View(user);
            }

            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Password", user.Password);

            switch (existingUser.UserRole)
            {
                case Role.Administrator:
                    HttpContext.Session.SetString("Role", "Administrator");
                    return RedirectToAction("Index", "Home");

                case Role.Patient:
                    HttpContext.Session.SetString("Role", "Patient");
                    return RedirectToAction("Index", "Home");

                case Role.HealthcareProfessional:
                    HttpContext.Session.SetString("Role", "HealthcareProfessional");
                    return RedirectToAction("Index", "Home");

                default:
                    return RedirectToAction("Create", "Appointment");
            }
        }

        // Method to verify the password
        private bool VerifyPassword(string enteredPassword, string storedPassword)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] enteredBytes = Encoding.UTF8.GetBytes(enteredPassword);
                byte[] enteredHash = sha256.ComputeHash(enteredBytes);
                string enteredHashString = Convert.ToBase64String(enteredHash);

                return storedPassword == enteredHashString;
            }
        }

        // GET: User/Dashboard
        public IActionResult Dashboard()
        {
            // Logic for the user's dashboard
            return View();
        }
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
