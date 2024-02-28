using PulseCare.Models.MedicalRecord;

namespace PulseCare.Interfaces;

public interface IMedicalRecordService
{
    ValueTask<bool> AddMedicalRecordAsync(MedicalRecordModel newMedicalRecord);
    ValueTask<bool> UpdateMedicalRecordAsync(int medicalRecordId, MedicalRecordUpdateModel updatedMedicalRecord);
    ValueTask<bool> RemoveMedicalRecordAsync(int medicalRecordId);
    ValueTask<MedicalRecordViewModel> GetMedicalRecordByIdAsync(int medicalRecordId);
    ValueTask<List<MedicalRecordViewModel>> GetAllMedicalRecordsAsync();
}