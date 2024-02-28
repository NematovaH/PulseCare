namespace PulseCare.Models.Appointment;

public class AppointmentUpdateModel
{
    public int DoctorId { get; set; }
    public int PatientId { get; set; }
    public DateTime Date { get; set; }
    public string Notes { get; set; }
}