using PulseCare.Models.Doctor;
using PulseCare.Models.Nurse;
using PulseCare.Models.Patient;

namespace PulseCare.Models.Hospital;

public class HospitalUpdateModel
{
    public string Name { get; set; }
    public decimal Balance { get; set; }
    public int RoomsCount { get; set; }
    public List<NurseModel> Nurses { get; set; } = new List<NurseModel>();
    public List<DoctorModel> Doctors { get; set; } = new List<DoctorModel>();
    public List<PatientModel> Patients { get; set; } = new List<PatientModel>();
}
