namespace PulseCare.Models.Appointment;

public class AppointmentViewModel
{
    public int Id { get; set; }
    public int DoctorId { get; set; }
    public int PatientId { get; set; }
    public DateTime Date { get; set; }
    public string Notes { get; set; }
}