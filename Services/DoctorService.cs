using PulseCare.Configuration;
using PulseCare.Extensions;
using PulseCare.Helpers;
using PulseCare.Interfaces;
using PulseCare.Models.Appointment;
using PulseCare.Models.Doctor;

namespace PulseCare.Services
{
    public class DoctorService : IDoctorService
    {
        private List<DoctorModel> doctors;

        public async ValueTask<DoctorViewModel> CreateDoctorAsync(DoctorCreationModel newDoctor)
        {
            doctors = await FileIO.ReadAsync<DoctorModel>(Constants.DOCTOR_PATH);

            if (doctors.Any(d => d.PhoneNumber == newDoctor.PhoneNumber))
            {
                throw new Exception("A doctor with the same phone number already exists.");
            }

            var createdDoctor = doctors.Create(newDoctor.MapTo<DoctorModel>());

            await FileIO.WriteAsync(Constants.DOCTOR_PATH, doctors);

            return createdDoctor.MapTo<DoctorViewModel>();
        }

        public async ValueTask<bool> DeleteDoctorAsync(int id)
        {
            doctors = await FileIO.ReadAsync<DoctorModel>(Constants.DOCTOR_PATH);

            var existingDoctor = doctors.FirstOrDefault(d => d.Id == id && !d.IsDeleted)
                ?? throw new Exception($"Doctor not found with this id -> {id}");

            existingDoctor.IsDeleted = true;
            existingDoctor.DeletedAt = DateTime.UtcNow;

            await FileIO.WriteAsync(Constants.DOCTOR_PATH, doctors);

            return true;
        }

        public async ValueTask<IEnumerable<DoctorViewModel>> GetAllDoctorsAsync()
        {
            doctors = await FileIO.ReadAsync<DoctorModel>(Constants.DOCTOR_PATH);

            var allDoctors = doctors
                .Where(d => !d.IsDeleted)
                .Select(d => new DoctorViewModel
                {
                    Id = d.Id,
                    FirstName = d.FirstName,
                    LastName = d.LastName,
                    PhoneNumber = d.PhoneNumber,
                    Salary = d.Salary,
                    Specialization = d.Specialization,
                    WorkStartTime = d.WorkStartTime,
                    WorkEndTime = d.WorkEndTime,
                    WorkingDays = d.WorkingDays,
                });

            return allDoctors;
        }

        public async ValueTask<DoctorViewModel> GetDoctorByIdAsync(int id)
        {
            doctors = await FileIO.ReadAsync<DoctorModel>(Constants.DOCTOR_PATH);

            var existingDoctor = doctors.FirstOrDefault(d => d.Id == id && !d.IsDeleted)
                ?? throw new Exception($"Doctor not found with this id -> {id}");

            return existingDoctor.MapTo<DoctorViewModel>();
        }

        public async ValueTask<IEnumerable<AppointmentViewModel>> GetAppointmentsAsync(int doctorId)
        {
            doctors = await FileIO.ReadAsync<DoctorModel>(Constants.DOCTOR_PATH);
            var appointments = await FileIO.ReadAsync<AppointmentModel>(Constants.APPOINTMENT_PATH);

            var doctorAppointments = appointments
                .Where(appointment => appointment.DoctorId == doctorId)
                .Where(appointment => !appointment.IsDeleted)
                .Select(appointment => new AppointmentViewModel
                {
                    Id = appointment.Id,
                    DoctorId = appointment.DoctorId,
                    PatientId = appointment.PatientId,
                    Date = appointment.Date,
                    Notes = appointment.Notes,
                });

            return doctorAppointments;
        }

        public async ValueTask<bool> UpdateDoctorAsync(int id, DoctorUpdateModel updatedDoctor)
        {
            doctors = await FileIO.ReadAsync<DoctorModel>(Constants.DOCTOR_PATH);

            var existingDoctor = doctors.FirstOrDefault(d => d.Id == id && !d.IsDeleted)
                ?? throw new Exception($"Doctor not found with this id -> {id}");

            if (doctors.Any(d => d.Id != id && d.PhoneNumber == updatedDoctor.PhoneNumber))
            {
                throw new Exception("Updated phone number already exists.");
            }

            existingDoctor.PhoneNumber = updatedDoctor.PhoneNumber;
            existingDoctor.WorkStartTime = updatedDoctor.WorkStartTime;
            existingDoctor.WorkEndTime = updatedDoctor.WorkEndTime;
            existingDoctor.WorkingDays = updatedDoctor.WorkingDays;

            await FileIO.WriteAsync(Constants.DOCTOR_PATH, doctors);

            return true;
        }

        public async ValueTask<bool> UpdatePasswordAsync(int id, string password)
        {
            doctors = await FileIO.ReadAsync<DoctorModel>(Constants.DOCTOR_PATH);

            var existingDoctor = doctors.FirstOrDefault(d => d.Id == id && !d.IsDeleted)
                ?? throw new Exception($"Doctor not found with this id -> {id}");

            existingDoctor.Password = password;
            existingDoctor.UpdatedAt = DateTime.UtcNow;

            await FileIO.WriteAsync(Constants.DOCTOR_PATH, doctors);

            return true;
        }

        public async ValueTask<List<string>> ViewMessageBox(int doctorId)
        {
            doctors = await FileIO.ReadAsync<DoctorModel>(Constants.DOCTOR_PATH);

            var existingDoctor = doctors.FirstOrDefault(d => d.Id == doctorId && !d.IsDeleted)
                ?? throw new Exception($"Doctor not found with this id -> {doctorId}");

            return existingDoctor.MapTo<DoctorViewModel>().MessageBox;
        }

        public async ValueTask<AppointmentViewModel> CreateAppointmentAsync(AppointmentCreationModel newAppointment)
        {
            var appointments = await FileIO.ReadAsync<AppointmentModel>(Constants.APPOINTMENT_PATH);

            var addedAppointment = appointments.Create(newAppointment.MapTo<AppointmentModel>());

            await FileIO.WriteAsync(Constants.APPOINTMENT_PATH, appointments);

            return addedAppointment.MapTo<AppointmentViewModel>();
        }

        public async ValueTask<bool> DeleteAppointmentAsync(int id)
        {
            var appointments = await FileIO.ReadAsync<AppointmentModel>(Constants.APPOINTMENT_PATH);

            var existingDoctor = doctors.FirstOrDefault(d => d.Id == id && !d.IsDeleted)
                ?? throw new Exception($"Appointment not found with this id -> {id}");

            existingDoctor.IsDeleted = true;
            existingDoctor.DeletedAt = DateTime.UtcNow;

            await FileIO.WriteAsync(Constants.DOCTOR_PATH, doctors);

            return true; ;
        }
    }
}
