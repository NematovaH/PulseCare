using PulseCare.Models.Hospital;

namespace PulseCare.Interfaces.Hospitals;

public interface IHospitalService
{
    ValueTask<HospitalViewModel> CreateHospitalAsync(HospitalCreationModel hospital);
    ValueTask<bool> DeleteAsync(int id);
    ValueTask<List<HospitalViewModel>> GetAllAsync();
    ValueTask<HospitalViewModel> GetByIdAsync(int id);
    ValueTask<bool> UpdateHospitalInfoAsync(int hospitalId, HospitalUpdateModel updatedHospital);
    ValueTask<bool> UpgradeBalance(int id, decimal amount);

}