using PulseCare.Configuration;
using PulseCare.Extensions;
using PulseCare.Helpers;
using PulseCare.Interfaces;
using PulseCare.Models.Doctor;
using PulseCare.Models.Hospital;
using PulseCare.Models.Nurse;

namespace PulseCare.Services
{
    public class NurseService : INurseService
    {
        private List<NurseModel> nurses;

        public async ValueTask<bool> AssignPatientAsync(int nurseId, int patientId)
        {
            nurses = await FileIO.ReadAsync<NurseModel>(Constants.NURSE_PATH);

            var existingNurse = nurses.FirstOrDefault(n => n.Id == nurseId && !n.IsDeleted)
                ?? throw new Exception($"Nurse not found with this id -> {nurseId}");

            existingNurse.AssignedPatientId = patientId;

            await FileIO.WriteAsync(Constants.NURSE_PATH, nurses);

            return true;
        }

        public async ValueTask<NurseViewModel> CreateNurseAsync(NurseCreationModel newNurse)
        {
            nurses = await FileIO.ReadAsync<NurseModel>(Constants.NURSE_PATH);

            if (nurses.Any(n => n.PhoneNumber == newNurse.PhoneNumber))
            {
                throw new Exception("A nurse with the same phone number already exists.");
            }

            var createdNurse = nurses.Create(newNurse.MapTo<NurseModel>());

            await FileIO.WriteAsync(Constants.NURSE_PATH, nurses);

            return createdNurse.MapTo<NurseViewModel>();
        }

        public async ValueTask<bool> DeleteNurseAsync(int id)
        {
            nurses = await FileIO.ReadAsync<NurseModel>(Constants.NURSE_PATH);

            var existingNurse = nurses.FirstOrDefault(n => n.Id == id && !n.IsDeleted)
                ?? throw new Exception($"Nurse not found with this id -> {id}");

            existingNurse.IsDeleted = true;
            existingNurse.DeletedAt = DateTime.UtcNow;

            await FileIO.WriteAsync(Constants.NURSE_PATH, nurses);

            return true;
        }

        public async ValueTask<IEnumerable<NurseViewModel>> GetAllNursesAsync()
        {
            nurses = await FileIO.ReadAsync<NurseModel>(Constants.NURSE_PATH);

            var allNurses = nurses
                .Where(n => !n.IsDeleted)
                .Select(n => new NurseViewModel
                {
                    Id = n.Id,
                    FirstName = n.FirstName,
                    LastName = n.LastName,
                    PhoneNumber = n.PhoneNumber,
                    Salary = n.Salary,
                    AssignedDoctorId = n.AssignedDoctorId,
                    AssignedPatientId = n.AssignedPatientId,
                });

            return allNurses;
        }

        public async ValueTask<NurseViewModel> GetNurseByIdAsync(int id)
        {
            nurses = await FileIO.ReadAsync<NurseModel>(Constants.NURSE_PATH);

            var existingNurse = nurses.FirstOrDefault(n => n.Id == id && !n.IsDeleted)
                ?? throw new Exception($"Nurse not found with this id -> {id}");

            return existingNurse.MapTo<NurseViewModel>();
        }

        public async ValueTask<bool> UnassignPatientAsync(int nurseId)
        {
            nurses = await FileIO.ReadAsync<NurseModel>(Constants.NURSE_PATH);

            var existingNurse = nurses.FirstOrDefault(n => n.Id == nurseId && !n.IsDeleted)
                ?? throw new Exception($"Nurse not found with this id -> {nurseId}");

            existingNurse.AssignedPatientId = 0;

            await FileIO.WriteAsync(Constants.NURSE_PATH, nurses);

            return true;
        }

        public async ValueTask<bool> UpdatePhoneAsync(int id, string phone)
        {
            nurses = await FileIO.ReadAsync<NurseModel>(Constants.NURSE_PATH);

            var existingNurse = nurses.FirstOrDefault(n => n.Id == id && !n.IsDeleted)
                ?? throw new Exception($"Nurse not found with this id -> {id}");

            if (nurses.Any(n => n.Id != id && n.PhoneNumber == phone))
            {
                throw new Exception("Updated phone number already exists.");
            }

            existingNurse.PhoneNumber = phone;
            existingNurse.UpdatedAt = DateTime.UtcNow;

            await FileIO.WriteAsync(Constants.NURSE_PATH, nurses);

            return true;
        }

        public async ValueTask<bool> UpdatePasswordAsync(int id, string password)
        {
            nurses = await FileIO.ReadAsync<NurseModel>(Constants.NURSE_PATH);

            var existingNurse = nurses.FirstOrDefault(n => n.Id == id && !n.IsDeleted)
                ?? throw new Exception($"Nurse not found with this id -> {id}");

            existingNurse.Password = password;
            existingNurse.UpdatedAt = DateTime.UtcNow;

            await FileIO.WriteAsync(Constants.NURSE_PATH, nurses);

            return true;
        }

        public async ValueTask<bool> UpdateSalaryAsync(int id, decimal salary)
        {
            nurses = await FileIO.ReadAsync<NurseModel>(Constants.NURSE_PATH);
            var hospital = await FileIO.ReadAsync<HospitalModel>(Constants.HOSPITAL_PATH);

            var existingNurse = nurses.FirstOrDefault(n => n.Id == id && !n.IsDeleted)
                ?? throw new Exception($"Nurse not found with this id -> {id}");

            var existingHospital = hospital.FirstOrDefault(p => p.Id == 1 && !p.IsDeleted)
                ?? throw new Exception($"Hospital not found with this id -> {id}");

            existingNurse.Salary += salary;
            existingHospital.Balance -= salary;
            existingHospital.UpdatedAt = DateTime.UtcNow;
            existingNurse.UpdatedAt = DateTime.UtcNow;

            await FileIO.WriteAsync(Constants.HOSPITAL_PATH, hospital);
            await FileIO.WriteAsync(Constants.NURSE_PATH, nurses);

            return true;
        }

        public async ValueTask<List<string>> ViewMessageBox(int nurseId)
        {
            nurses = await FileIO.ReadAsync<NurseModel>(Constants.NURSE_PATH);

            var existingNurse = nurses.FirstOrDefault(d => d.Id == nurseId && !d.IsDeleted)
                ?? throw new Exception($"Nurse not found with this id -> {nurseId}");

            return existingNurse.MapTo<NurseViewModel>().MessageBox;
        }
    }
}
