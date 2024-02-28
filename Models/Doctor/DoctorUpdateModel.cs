using PulseCare.Enums;

namespace PulseCare.Models.Doctor;

public class DoctorUpdateModel
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public decimal Salary { get; set; }
    public DoctorSpecialization Specialization { get; set; }
    public TimeSpan WorkStartTime { get; set; }
    public TimeSpan WorkEndTime { get; set; }
    public WorkDays WorkingDays { get; set; }
    public string Password { get; set; }
    public List<string> MessageBox { get; set; } = new List<string>();

}