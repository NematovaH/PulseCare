using PulseCare.Configuration;
using PulseCare.Extensions;
using PulseCare.Helpers;
using PulseCare.Interfaces;
using PulseCare.Models.MedicalRecord;

namespace PulseCare.Services
{
    public class MedicalRecordService : IMedicalRecordService
    {
        private List<MedicalRecordModel> medicalRecords;

        public async ValueTask<bool> AddMedicalRecordAsync(MedicalRecordModel newMedicalRecord)
        {
            medicalRecords = await FileIO.ReadAsync<MedicalRecordModel>(Constants.MEDICAL_RECORD_PATH);

            var addedMedicalRecord = medicalRecords.Create(newMedicalRecord);

            await FileIO.WriteAsync(Constants.MEDICAL_RECORD_PATH, medicalRecords);

            return true;
        }

        public async ValueTask<List<MedicalRecordViewModel>> GetAllMedicalRecordsAsync()
        {
            medicalRecords = await FileIO.ReadAsync<MedicalRecordModel>(Constants.MEDICAL_RECORD_PATH);

            var allMedicalRecords = medicalRecords
                .Where(mr => !mr.IsDeleted)
                .Select(mr => new MedicalRecordViewModel
                {
                    Id = mr.Id,
                    DoctorId = mr.DoctorId,
                    PatientId = mr.PatientId,
                    Diagnosis = mr.Diagnosis,
                    Medication = mr.Medication,
                    DatePrescribed = mr.DatePrescribed,
                })
                .ToList();

            return allMedicalRecords;
        }

        public async ValueTask<MedicalRecordViewModel> GetMedicalRecordByIdAsync(int medicalRecordId)
        {
            medicalRecords = await FileIO.ReadAsync<MedicalRecordModel>(Constants.MEDICAL_RECORD_PATH);

            var existingMedicalRecord = medicalRecords.FirstOrDefault(mr => mr.Id == medicalRecordId && !mr.IsDeleted)
                ?? throw new Exception($"Medical Record not found with this id -> {medicalRecordId}");

            var medicalRecordViewModel = new MedicalRecordViewModel
            {
                Id = existingMedicalRecord.Id,
                DoctorId = existingMedicalRecord.DoctorId,
                PatientId = existingMedicalRecord.PatientId,
                Diagnosis = existingMedicalRecord.Diagnosis,
                Medication = existingMedicalRecord.Medication,
                DatePrescribed = existingMedicalRecord.DatePrescribed,
            };

            return medicalRecordViewModel;
        }

        public async ValueTask<bool> RemoveMedicalRecordAsync(int medicalRecordId)
        {
            medicalRecords = await FileIO.ReadAsync<MedicalRecordModel>(Constants.MEDICAL_RECORD_PATH);

            var existingMedicalRecord = medicalRecords.FirstOrDefault(mr => mr.Id == medicalRecordId && !mr.IsDeleted)
                ?? throw new Exception($"Medical Record not found with this id -> {medicalRecordId}");

            existingMedicalRecord.IsDeleted = true;
            existingMedicalRecord.DeletedAt = DateTime.UtcNow;

            await FileIO.WriteAsync(Constants.MEDICAL_RECORD_PATH, medicalRecords);

            return true;
        }

        public async ValueTask<bool> UpdateMedicalRecordAsync(int medicalRecordId, MedicalRecordUpdateModel updatedMedicalRecord)
        {
            medicalRecords = await FileIO.ReadAsync<MedicalRecordModel>(Constants.MEDICAL_RECORD_PATH);

            var existingMedicalRecord = medicalRecords.FirstOrDefault(mr => mr.Id == medicalRecordId && !mr.IsDeleted)
                ?? throw new Exception($"Medical Record not found with this id -> {medicalRecordId}");

            existingMedicalRecord.Diagnosis = updatedMedicalRecord.Diagnosis;
            existingMedicalRecord.Medication = updatedMedicalRecord.Medication;
            existingMedicalRecord.DatePrescribed = updatedMedicalRecord.DatePrescribed;
            existingMedicalRecord.UpdatedAt = DateTime.UtcNow;

            await FileIO.WriteAsync(Constants.MEDICAL_RECORD_PATH, medicalRecords);

            return true;
        }
    }
}
