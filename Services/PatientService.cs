using PulseCare.Configuration;
using PulseCare.Extensions;
using PulseCare.Helpers;
using PulseCare.Interfaces;
using PulseCare.Models.Appointment;
using PulseCare.Models.Hospital;
using PulseCare.Models.Patient;

namespace PulseCare.Services
{
    public class PatientService : IPatientService
    {
        private List<PatientModel> patients;

        public async ValueTask<PatientViewModel> CreatePatientAsync(PatientCreationModel newPatient)
        {
            patients = await FileIO.ReadAsync<PatientModel>(Constants.PATIENT_PATH);

            if (patients.Any(p => p.PhoneNumber == newPatient.PhoneNumber || p.Email == newPatient.Email))
            {
                throw new Exception("Patient with the same phone number or email already exists.");
            }

            var createdPatient = patients.Create(newPatient.MapTo<PatientModel>());

            await FileIO.WriteAsync(Constants.PATIENT_PATH, patients);

            return createdPatient.MapTo<PatientViewModel>();
        }

        public async ValueTask<bool> DeletePatientAsync(int id)
        {
            patients = await FileIO.ReadAsync<PatientModel>(Constants.PATIENT_PATH);

            var existingPatient = patients.FirstOrDefault(p => p.Id == id && !p.IsDeleted)
                ?? throw new Exception($"Patient not found with this id -> {id}");

            existingPatient.IsDeleted = true;
            existingPatient.DeletedAt = DateTime.UtcNow;

            await FileIO.WriteAsync(Constants.PATIENT_PATH, patients);

            return true;
        }

        public async ValueTask<IEnumerable<PatientViewModel>> GetAllPatientsAsync()
        {
            patients = await FileIO.ReadAsync<PatientModel>(Constants.PATIENT_PATH);

            var allPatients = patients
                .Where(p => !p.IsDeleted)
                .Select(p => new PatientViewModel
                {
                    Id = p.Id,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    Gender = p.Gender,
                    DateOfBirth = p.DateOfBirth,
                    PhoneNumber = p.PhoneNumber,
                    Email = p.Email,
                    Address = p.Address,
                    Password = p.Password,
                    Balance = p.Balance,
                    AssignedDoctorId = p.AssignedDoctorId,
                    AssignedNurseId = p.AssignedNurseId,
                    Status = p.Status,
                });

            return allPatients;
        }

        public async ValueTask<PatientViewModel> GetPatientByIdAsync(int id)
        {
            patients = await FileIO.ReadAsync<PatientModel>(Constants.PATIENT_PATH);

            var existingPatient = patients.FirstOrDefault(p => p.Id == id && !p.IsDeleted)
                ?? throw new Exception($"Patient not found with this id -> {id}");

            return existingPatient.MapTo<PatientViewModel>();
        }

        public async ValueTask<IEnumerable<AppointmentViewModel>> GetAppointmentsAsync(int patientId)
        {
            patients = await FileIO.ReadAsync<PatientModel>(Constants.PATIENT_PATH);
            var appointments = await FileIO.ReadAsync<AppointmentModel>(Constants.APPOINTMENT_PATH);

            var patientAppointments = appointments
                .Where(appointment => appointment.PatientId == patientId)
                .Where(appointment => !appointment.IsDeleted)
                .Select(appointment => new AppointmentViewModel
                {
                    Id = appointment.Id,
                    DoctorId = appointment.DoctorId,
                    PatientId = appointment.PatientId,
                    Date = appointment.Date,
                    Notes = appointment.Notes,
                });

            return patientAppointments;
        }

        public async ValueTask<bool> UpdatePasswordAsync(int id, string password)
        {
            patients = await FileIO.ReadAsync<PatientModel>(Constants.PATIENT_PATH);

            var existingPatient = patients.FirstOrDefault(p => p.Id == id && !p.IsDeleted)
                ?? throw new Exception($"Patient not found with this id -> {id}");

            existingPatient.Password = password;
            existingPatient.UpdatedAt = DateTime.UtcNow;

            await FileIO.WriteAsync(Constants.PATIENT_PATH, patients);

            return true;
        }

        public async ValueTask<bool> UpdatePatientAsync(int id, PatientUpdateModel updatedPatient)
        {
            patients = await FileIO.ReadAsync<PatientModel>(Constants.PATIENT_PATH);

            var existingPatient = patients.FirstOrDefault(p => p.Id == id && !p.IsDeleted)
                ?? throw new Exception($"Patient not found with this id -> {id}");

            if (patients.Any(p => p.Id != id && (p.PhoneNumber == updatedPatient.PhoneNumber || p.Email == updatedPatient.Email)))
            {
                throw new Exception("Updated phone number or email already exists.");
            }

            existingPatient.PhoneNumber = updatedPatient.PhoneNumber;
            existingPatient.Email = updatedPatient.Email;
            existingPatient.Address = updatedPatient.Address;
            existingPatient.UpdatedAt = DateTime.UtcNow;

            await FileIO.WriteAsync(Constants.PATIENT_PATH, patients);

            return true;
        }

        public async ValueTask<bool> UpgradeBalanceAsync(int id, decimal amount)
        {
            patients = await FileIO.ReadAsync<PatientModel>(Constants.PATIENT_PATH);

            var existingPatient = patients.FirstOrDefault(p => p.Id == id && !p.IsDeleted)
                ?? throw new Exception($"Patient not found with this id -> {id}");

            existingPatient.Balance += amount;
            existingPatient.UpdatedAt = DateTime.UtcNow;

            await FileIO.WriteAsync(Constants.PATIENT_PATH, patients);

            return true;
        }

        public async ValueTask<bool> DegradeBalanceAsync(int id, decimal amount)
        {
            patients = await FileIO.ReadAsync<PatientModel>(Constants.PATIENT_PATH);
            var hospital = await FileIO.ReadAsync<HospitalModel>(Constants.HOSPITAL_PATH);


            var existingHospital = hospital.FirstOrDefault(p => p.Id == 1 && !p.IsDeleted)
                ?? throw new Exception($"Hospital not found with this id -> {id}");


            var existingPatient = patients.FirstOrDefault(p => p.Id == id && !p.IsDeleted)
                ?? throw new Exception($"Patient not found with this id -> {id}");

            if (existingPatient.Balance >= amount)
            {
                existingPatient.Balance -= amount;
                existingHospital.Balance += amount;
                existingPatient.UpdatedAt = DateTime.UtcNow;
                await FileIO.WriteAsync(Constants.HOSPITAL_PATH, hospital);
                await FileIO.WriteAsync(Constants.PATIENT_PATH, patients);
                return true;
            }
            else
            {
                throw new Exception("Insufficient balance to degrade.");
            }
        }

        public async ValueTask<IEnumerable<PatientViewModel>> SearchAsync(string keyword)
        {
            patients = await FileIO.ReadAsync<PatientModel>(Constants.PATIENT_PATH);

            var searchResults = patients
                .Where(patient =>
                    patient.FirstName.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    patient.LastName.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    patient.PhoneNumber.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    patient.Email.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                .Select(patient => new PatientViewModel
                {
                    Id = patient.Id,
                    FirstName = patient.FirstName,
                    LastName = patient.LastName,
                    PhoneNumber = patient.PhoneNumber,
                    Email = patient.Email,
                });

            return searchResults;
        }
    }
}
