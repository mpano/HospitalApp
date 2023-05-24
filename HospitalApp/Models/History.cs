namespace HospitalApp.Models
{
    public class History
    {
        public int Id { get; set; }
        public int PatientID { get; set; }
        public DateTime VisitDate { get; set; }
        public string Diagnosis { get; set; }
        public string Treatment { get; set; }
        public string Medications { get; set; }
        public string Notes { get; set; }
        public string PatientEmail { get; set; }
    }
}
