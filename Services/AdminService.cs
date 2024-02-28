using PulseCare.Configuration;
using PulseCare.Helpers;
using PulseCare.Interfaces;
using PulseCare.Models.Admin;

namespace PulseCare.Services;

public class AdminService : IAdminService
{
    private List<AdminModel> admins;

    public async ValueTask<bool> UpdatePasswordAsync(string password)
    {
        admins = await FileIO.ReadAsync<AdminModel>(Constants.ADMIN_PATH);

        var existingPerson = admins.FirstOrDefault(p => p.Id == 1 && !p.IsDeleted)
            ?? throw new Exception($"Admin not found with this id -> {1}");

        existingPerson.Password = password;
        existingPerson.UpdatedAt = DateTime.UtcNow;

        await FileIO.WriteAsync(Constants.ADMIN_PATH, admins);

        return true;
    }
}