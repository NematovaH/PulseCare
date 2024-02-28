using PulseCare.Models.Appointment;
using PulseCare.Models.Patient;

namespace PulseCare.Interfaces;

public interface IPatientService
{
    ValueTask<bool> DeletePatientAsync(int id);
    ValueTask<IEnumerable<PatientViewModel>> GetAllPatientsAsync();
    ValueTask<PatientViewModel> CreatePatientAsync(PatientCreationModel newPatient);
    ValueTask<bool> UpdatePatientAsync(int id, PatientUpdateModel updatedPatient);
    ValueTask<bool> UpgradeBalanceAsync(int id, decimal salary);
    ValueTask<bool> DegradeBalanceAsync(int id, decimal salary);
    ValueTask<PatientViewModel> GetPatientByIdAsync(int id);
    ValueTask<IEnumerable<AppointmentViewModel>> GetAppointmentsAsync(int patientId);
    ValueTask<bool> UpdatePasswordAsync(int id, string password);
    ValueTask<IEnumerable<PatientViewModel>> SearchAsync(string keyword);
}