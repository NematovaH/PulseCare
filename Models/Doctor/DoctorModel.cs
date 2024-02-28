using PulseCare.Enums;
using PulseCare.Models.Commons;

namespace PulseCare.Models.Doctor;

public class DoctorModel : Auditable
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public decimal Salary { get; set; }
    public DoctorSpecialization Specialization { get; set; }
    public TimeSpan WorkStartTime { get; set; }
    public TimeSpan WorkEndTime { get; set; }
    public string Password { get; set; }
    public WorkDays WorkingDays { get; set; }
    public List<string> MessageBox { get; set; } = new List<string>();
}