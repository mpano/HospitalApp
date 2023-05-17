using System.ComponentModel.DataAnnotations;

namespace HospitalApp.Models
{
    public class User
    {
        public int UserID { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; }

        [Required]
        [StringLength(100)]
        public string Password { get; set; }

        public Role UserRole { get; set; }

        public User()
        {
            UserRole = Role.Patient; // Set default role to Patient
        }
    }

    public enum Role
    {
        Administrator,
        HealthcareProfessional,
        Patient
    }

}
