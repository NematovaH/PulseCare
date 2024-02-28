using PulseCare.Models.Commons;

namespace PulseCare.Models.Nurse;

public class NurseModel : Auditable
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
    public decimal Salary { get; set; }
    public int AssignedDoctorId { get; set; }
    public int AssignedPatientId { get; set; } = 0;
    public List<string> MessageBox { get; set; } = new List<string>();

}