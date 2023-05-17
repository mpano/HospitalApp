using System.ComponentModel.DataAnnotations;

namespace HospitalApp.Models
{
    public class HealthcareSpecialist
    {
        [Key]
        public int HealthcareProviderID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Specialization { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
    }

}
