using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalApp.Models.Dto
{
    [NotMapped]
    public class PatientDto
    {
        public string fullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
