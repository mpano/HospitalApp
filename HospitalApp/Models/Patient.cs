using ServiceStack.DataAnnotations;

namespace HospitalApp.Models
{
    public class Patient
    {
        public int id { get; set; }
        public string fullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        [Unique]
        public string? Email { get; set; }
        public string? InsuranceProvider { get; set; }
        public string? InsurancePolicyNumber { get; set; }

    }
}
