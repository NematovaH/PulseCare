using PulseCare.Models.Accountant;

namespace PulseCare.Interfaces;

public interface IAccountantService
{
    ValueTask<bool> DeleteAccountantAsync(int id);
    ValueTask<IEnumerable<AccountantViewModel>> GetAllAccountantsAsync();
    ValueTask<AccountantViewModel> CreateAccountantAsync(AccountantCreationModel newAccountant);
    ValueTask<AccountantViewModel> GetAccountantByIdAsync(int id);
    ValueTask<bool> UpdatePasswordAsync(int id, string password);
    ValueTask<bool> DistributeSalaryToWorkersAsync();
}