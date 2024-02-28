using PulseCare.Configuration;
using PulseCare.Extensions;
using PulseCare.Helpers;
using PulseCare.Interfaces.Hospitals;
using PulseCare.Models.Doctor;
using PulseCare.Models.Hospital;
using PulseCare.Models.Nurse;
using PulseCare.Models.Patient;

namespace PulseCare.Services
{
    public class HospitalService : IHospitalService
    {
        private List<HospitalModel> hospitals;

        public async ValueTask<HospitalViewModel> CreateHospitalAsync(HospitalCreationModel hospital)
        {
            hospitals = await FileIO.ReadAsync<HospitalModel>(Constants.HOSPITAL_PATH);

            if (hospitals.Any(h => h.Name == hospital.Name))
            {
                throw new Exception($"Hospital with the name '{hospital.Name}' already exists.");
            }

            var newHospital = new HospitalModel
            {
                Name = hospital.Name,
                RoomsCount = hospital.RoomsCount,
                Balance = hospital.Balance,
                Nurses = hospital.Nurses?.Select(nurse => nurse.MapTo<NurseModel>()).ToList(),
                Doctors = hospital.Doctors?.Select(doctor => doctor.MapTo<DoctorModel>()).ToList(),
                Patients = hospital.Patients?.Select(patient => patient.MapTo<PatientModel>()).ToList()
            };

            var createdHospital = hospitals.Create(newHospital);

            await FileIO.WriteAsync(Constants.HOSPITAL_PATH, hospitals);

            return createdHospital.MapTo<HospitalViewModel>();
        }

        public async ValueTask<bool> DeleteAsync(int id)
        {
            hospitals = await FileIO.ReadAsync<HospitalModel>(Constants.HOSPITAL_PATH);

            var existingHospital = hospitals.FirstOrDefault(d => d.Id == id && !d.IsDeleted)
                ?? throw new Exception($"Hospital not found with this id -> {id}");

            existingHospital.IsDeleted = true;
            existingHospital.DeletedAt = DateTime.UtcNow;

            await FileIO.WriteAsync(Constants.HOSPITAL_PATH, hospitals);

            return true;
        }

        public async ValueTask<List<HospitalViewModel>> GetAllAsync()
        {
            hospitals = await FileIO.ReadAsync<HospitalModel>(Constants.HOSPITAL_PATH);
            var allHospitals = hospitals
                .Where(d => !d.IsDeleted)
                .Select(d => new HospitalViewModel
                {
                    Id = d.Id,
                    Name = d.Name,
                    Balance = d.Balance,
                    RoomsCount = d.RoomsCount,
                });

            return hospitals.MapTo<HospitalViewModel>();
        }

        public async ValueTask<HospitalViewModel> GetByIdAsync(int id)
        {
            hospitals = await FileIO.ReadAsync<HospitalModel>(Constants.HOSPITAL_PATH);

            var existingHospital = hospitals.FirstOrDefault(d => d.Id == id && !d.IsDeleted)
                ?? throw new Exception($"Hospital not found with this id -> {id}");

            return existingHospital.MapTo<HospitalViewModel>();
        }

        public async ValueTask<bool> UpgradeBalance(int id, decimal amount)
        {
            hospitals = await FileIO.ReadAsync<HospitalModel>(Constants.HOSPITAL_PATH);

            var hospital = hospitals.FirstOrDefault(h => h.Id == id && !h.IsDeleted)
                ?? throw new Exception($"Hospital not found with this id -> {id}");
            hospital.Balance += amount;
            hospital.UpdatedAt = DateTime.UtcNow;

            await FileIO.WriteAsync(Constants.HOSPITAL_PATH, hospitals);

            return true;
        }

        public async ValueTask<bool> UpdateHospitalInfoAsync(int hospitalId, HospitalUpdateModel updatedHospital)
        {
            hospitals = await FileIO.ReadAsync<HospitalModel>(Constants.HOSPITAL_PATH);

            var hospital = hospitals.FirstOrDefault(h => h.Id == hospitalId && !h.IsDeleted)
                ?? throw new Exception($"Hospital not found with this id -> {hospitalId}");

            hospital.Name = updatedHospital.Name;
            hospital.RoomsCount = updatedHospital.RoomsCount;
            hospital.UpdatedAt = DateTime.UtcNow;

            await FileIO.WriteAsync(Constants.HOSPITAL_PATH, hospitals);

            return true;
        }
    }
}
