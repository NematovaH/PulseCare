namespace PulseCare.Interfaces
{
    public interface IAdminService
    {
        ValueTask<bool> UpdatePasswordAsync(string password);
    }
}