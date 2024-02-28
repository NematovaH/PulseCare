using PulseCare.Configuration;
using PulseCare.Extensions;
using PulseCare.Helpers;
using PulseCare.Interfaces;
using PulseCare.Models.Accountant;
using PulseCare.Models.Doctor;
using PulseCare.Models.Hospital;
using PulseCare.Models.Nurse;

namespace PulseCare.Services
{
    public class AccountantService : IAccountantService
    {
        private List<AccountantModel> accountants;

        public async ValueTask<AccountantViewModel> CreateAccountantAsync(AccountantCreationModel newAccountant)
        {
            accountants = await FileIO.ReadAsync<AccountantModel>(Constants.ACCOUNTANT_PATH);

            if (accountants.Any(a => a.PhoneNumber == newAccountant.PhoneNumber))
            {
                throw new Exception("An accountant with the same phone number already exists.");
            }

            var createdAccountant = accountants.Create(newAccountant.MapTo<AccountantModel>());

            await FileIO.WriteAsync(Constants.ACCOUNTANT_PATH, accountants);

            return createdAccountant.MapTo<AccountantViewModel>();
        }

        public async ValueTask<bool> DeleteAccountantAsync(int id)
        {
            accountants = await FileIO.ReadAsync<AccountantModel>(Constants.ACCOUNTANT_PATH);

            var existingAccountant = accountants.FirstOrDefault(a => a.Id == id && !a.IsDeleted)
                ?? throw new Exception($"Accountant not found with this id -> {id}");

            existingAccountant.IsDeleted = true;
            existingAccountant.DeletedAt = DateTime.UtcNow;

            await FileIO.WriteAsync(Constants.ACCOUNTANT_PATH, accountants);

            return true;
        }

        public async ValueTask<IEnumerable<AccountantViewModel>> GetAllAccountantsAsync()
        {
            accountants = await FileIO.ReadAsync<AccountantModel>(Constants.ACCOUNTANT_PATH);

            var allAccountants = accountants
                .Where(a => !a.IsDeleted)
                .Select(a => new AccountantViewModel
                {
                    Id = a.Id,
                    FirstName = a.FirstName,
                    LastName = a.LastName,
                    PhoneNumber = a.PhoneNumber,
                    Salary = a.Salary,
                });

            return allAccountants;
        }

        public async ValueTask<AccountantViewModel> GetAccountantByIdAsync(int id)
        {
            accountants = await FileIO.ReadAsync<AccountantModel>(Constants.ACCOUNTANT_PATH);

            var existingAccountant = accountants.FirstOrDefault(a => a.Id == id && !a.IsDeleted)
                ?? throw new Exception($"Accountant not found with this id -> {id}");

            return existingAccountant.MapTo<AccountantViewModel>();
        }

        public async ValueTask<bool> UpdatePhoneNumberAsync(int id, string phone)
        {
            accountants = await FileIO.ReadAsync<AccountantModel>(Constants.ACCOUNTANT_PATH);

            var existingAccountant = accountants.FirstOrDefault(a => a.Id == id && !a.IsDeleted)
                ?? throw new Exception($"Accountant not found with this id -> {id}");

            if (accountants.Any(a => a.Id != id && a.PhoneNumber == phone))
            {
                throw new Exception("Updated phone number already exists.");
            }

            existingAccountant.PhoneNumber = phone;
            existingAccountant.UpdatedAt = DateTime.UtcNow;

            await FileIO.WriteAsync(Constants.ACCOUNTANT_PATH, accountants);

            return true;
        }

        public async ValueTask<bool> UpdatePasswordAsync(int id, string password)
        {
            accountants = await FileIO.ReadAsync<AccountantModel>(Constants.ACCOUNTANT_PATH);

            var existingAccountant = accountants.FirstOrDefault(a => a.Id == id && !a.IsDeleted)
                ?? throw new Exception($"Accountant not found with this id -> {id}");

            existingAccountant.Password = password;
            existingAccountant.UpdatedAt = DateTime.UtcNow;

            await FileIO.WriteAsync(Constants.ACCOUNTANT_PATH, accountants);

            return true;
        }

        public async ValueTask<bool> DistributeSalaryToWorkersAsync()
        {
            try
            {
                var hospital = await FileIO.ReadAsync<HospitalModel>(Constants.HOSPITAL_PATH);

                var existingHospital = hospital.FirstOrDefault(p => p.Id == 1 && !p.IsDeleted)
                    ?? throw new Exception($"Hospital not found with this id -> {1}");

                var nurses = await FileIO.ReadAsync<NurseModel>(Constants.NURSE_PATH);
                var doctors = await FileIO.ReadAsync<DoctorModel>(Constants.DOCTOR_PATH);
                accountants = await FileIO.ReadAsync<AccountantModel>(Constants.ACCOUNTANT_PATH);

                foreach (var a in accountants)
                {
                    a.Salary += 3000;
                    existingHospital.Balance -= 3000;
                    a.MessageBox.Add($"Salary has been deposited to your account card, Accountant ID: {a.Id}");
                    a.UpdatedAt = DateTime.UtcNow;
                    existingHospital.UpdatedAt = DateTime.UtcNow;
                }

                foreach (var nurse in nurses)
                {
                    nurse.Salary += 2000;
                    existingHospital.Balance -= 2000;
                    nurse.MessageBox.Add($"Salary has been deposited to your account card, Nurse ID: {nurse.Id}");
                    nurse.UpdatedAt = DateTime.UtcNow;
                    existingHospital.UpdatedAt = DateTime.UtcNow;
                }

                foreach (var doctor in doctors)
                {
                    doctor.Salary += 4000;
                    existingHospital.Balance -= 4000;
                    doctor.MessageBox.Add($"Salary has been deposited to your account card, Doctor ID: {doctor.Id}");
                    doctor.UpdatedAt = DateTime.UtcNow;
                    existingHospital.UpdatedAt = DateTime.UtcNow;
                    existingHospital.UpdatedAt = DateTime.UtcNow;
                }

                await FileIO.WriteAsync(Constants.NURSE_PATH, nurses);
                await FileIO.WriteAsync(Constants.DOCTOR_PATH, doctors);
                await FileIO.WriteAsync(Constants.ACCOUNTANT_PATH, accountants);
                await FileIO.WriteAsync(Constants.HOSPITAL_PATH, hospital);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error distributing salary: {ex.Message}");
                return false;
            }
        }
    }
}
