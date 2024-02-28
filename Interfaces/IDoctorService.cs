using PulseCare.Models.Appointment;
using PulseCare.Models.Doctor;

namespace PulseCare.Interfaces;

public interface IDoctorService
{
    ValueTask<IEnumerable<DoctorViewModel>> GetAllDoctorsAsync();
    ValueTask<bool> DeleteDoctorAsync(int id);
    ValueTask<DoctorViewModel> CreateDoctorAsync(DoctorCreationModel newDoctor);
    ValueTask<AppointmentViewModel> CreateAppointmentAsync(AppointmentCreationModel newAppointment);
    ValueTask<bool> DeleteAppointmentAsync(int id);
    ValueTask<bool> UpdateDoctorAsync(int id, DoctorUpdateModel updatedDoctor);
    ValueTask<bool> UpdatePasswordAsync(int id, string password);
    ValueTask<DoctorViewModel> GetDoctorByIdAsync(int id);
    ValueTask<IEnumerable<AppointmentViewModel>> GetAppointmentsAsync(int doctorId);
    ValueTask<List<string>> ViewMessageBox(int doctorId);
}
