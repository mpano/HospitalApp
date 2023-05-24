using System.ComponentModel.DataAnnotations;

namespace HospitalApp.Models
{
    public class Appoint
    {
        [Key]
        public int AppointmentID { get; set; }
        public int PatientID { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public string AppointmentType { get; set; }
        public int HealthcareProviderID { get; set; }
        public string Notes { get; set; }
        public bool Approved { get; internal set; }
    }
}
