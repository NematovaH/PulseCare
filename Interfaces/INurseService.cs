using PulseCare.Models.Nurse;

namespace PulseCare.Interfaces;

public interface INurseService
{
    ValueTask<IEnumerable<NurseViewModel>> GetAllNursesAsync();
    ValueTask<NurseViewModel> CreateNurseAsync(NurseCreationModel newDoctor);
    ValueTask<bool> DeleteNurseAsync(int id);
    ValueTask<bool> UpdatePhoneAsync(int id, string phone);
    ValueTask<NurseViewModel> GetNurseByIdAsync(int id);
    ValueTask<bool> AssignPatientAsync(int nurseId, int patientId);
    ValueTask<bool> UnassignPatientAsync(int nurseId);
    ValueTask<bool> UpdatePasswordAsync(int id, string password);
    ValueTask<List<string>> ViewMessageBox(int nurseId);
}