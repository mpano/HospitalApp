namespace HospitalApp.Models
{
    public class MedicalHistory
    {
        public int MedicalHistoryID { get; set; }
        public int PatientID { get; set; }
        public DateTime VisitDate { get; set; }
        public string Diagnosis { get; set; }
        public string Treatment { get; set; }
        public string Medications { get; set; }
        public string Notes { get; set; }

        // Navigation property for the Patient entity
        public Patient Patient { get; set; }
    }
}
