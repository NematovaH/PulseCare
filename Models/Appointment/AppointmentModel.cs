using PulseCare.Models.Commons;

namespace PulseCare.Models.Appointment;

public class AppointmentModel : Auditable
{
    public int DoctorId { get; set; }
    public int PatientId { get; set; }
    public DateTime Date { get; set; }
    public string Notes { get; set; }
}