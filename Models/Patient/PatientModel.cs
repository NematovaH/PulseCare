using PulseCare.Enums;
using PulseCare.Models.Commons;

namespace PulseCare.Models.Patient;

public class PatientModel : Auditable
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Gender { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public string Password { get; set; }
    public decimal Balance { get; set; }
    public int AssignedDoctorId { get; set; }
    public int AssignedNurseId { get; set; }
    public PatientStatus Status { get; set; }
}
