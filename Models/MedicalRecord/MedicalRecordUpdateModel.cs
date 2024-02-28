namespace PulseCare.Models.MedicalRecord;

public class MedicalRecordUpdateModel
{
    public int DoctorId { get; set; }
    public int PatientId { get; set; }
    public string Diagnosis { get; set; }
    public string Medication { get; set; }
    public DateTime DatePrescribed { get; set; }
}