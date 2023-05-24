using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HospitalApp.Models;

namespace HospitalApp.Data
{
    public class HospitalAppContext : DbContext
    {
        public HospitalAppContext (DbContextOptions<HospitalAppContext> options)
            : base(options)
        {
        }

        public DbSet<HospitalApp.Models.Patient> Patient { get; set; } = default!;

        public DbSet<HospitalApp.Models.MedicalHistory>? MedicalHistory { get; set; }

        public DbSet<HospitalApp.Models.History>? History { get; set; }

        public DbSet<HospitalApp.Models.Appointment>? Appointment { get; set; }

        public DbSet<HospitalApp.Models.HealthcareSpecialist>? HealthcareSpecialist { get; set; }
        public DbSet<HospitalApp.Models.User>? Users { get; set; }
        public DbSet<HospitalApp.Models.MedicalHist>? MedicalHist { get; set; }
        public DbSet<HospitalApp.Models.Appoint>? Appoint { get; set; }
    }
}
