using PulseCare.Extensions;
using PulseCare.Models.Appointment;
using PulseCare.Models.Doctor;
using PulseCare.Models.MedicalRecord;
using PulseCare.Services;
using Spectre.Console;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace PulseCare.Menu
{
    public class DoctorMenu
    {
        private readonly DoctorService _doctorService;
        private readonly MedicalRecordService _medicalRecordService;
        private readonly PatientService _patientService;
        public DoctorMenu(DoctorService doctorService, MedicalRecordService medicalRecordService, PatientService patientService)
        {
            _doctorService = doctorService;
            _medicalRecordService = medicalRecordService;
            _patientService = patientService;
        }

        public void Login(int doctorId)
        {
            var rule = new Rule("\n[red]Doctor Menu[/]");
            AnsiConsole.Write(rule);
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(4)
                    .AddChoices(new[] { "My Profile", "Update Details", "Appointments", "Medical Records", "Salary Notification Box", "Exit" }));

            switch (choice)
            {
                case "My Profile":
                    AnsiConsole.Clear();
                    MyProfile(doctorId);
                    break;
                case "Update Details":
                    AnsiConsole.Clear();
                    UpdateDetails(doctorId);
                    break;
                case "Appointments":
                    AnsiConsole.Clear();
                    Appointments(doctorId);
                    break;
                case "Medical Records":
                    AnsiConsole.Clear();
                    MedicalRecording(doctorId);
                    break;
                case "Salary Notification Box":
                    AnsiConsole.Clear();
                    SalaryNotificationBox(doctorId);
                    break;
                case "Exit":
                    AnsiConsole.Clear();
                    AnsiConsole.WriteLine("Exiting Doctor Menu.");
                    return;
                default:
                    AnsiConsole.Clear();
                    AnsiConsole.WriteLine("Invalid choice. Please enter a valid option.");
                    break;
            }

            Login(doctorId);
        }

        private void MyProfile(int doctorId)
        {
            try
            {
                var doctor = _doctorService.GetDoctorByIdAsync(doctorId).Result;
                DisplayDoctorDetails(doctor);
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteLine($"Something went wrong - > {ex.Message}");
            }
        }

        private void SalaryNotificationBox(int nurseId)
        {
            try
            {
                var messages = _doctorService.ViewMessageBox(nurseId).Result;

                if (messages.Count == 0)
                {
                    AnsiConsole.WriteLine("No messages in the Salary Notification Box.");
                }
                else
                {
                    AnsiConsole.WriteLine("Salary Notification Box");
                    var table = new Table();
                    table.AddColumn("Messages");

                    foreach (var message in messages)
                    {
                        table.AddRow(message);
                    }

                    AnsiConsole.Write(table);
                }

                AnsiConsole.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteLine($"Something went wrong - > {ex.Message}");

                AnsiConsole.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }


        private void UpdateDetails(int doctorId)
        {
            var updateChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(3)
                    .AddChoices(new[] { "Update Phone", "Update Password", "Back" }));

            switch (updateChoice)
            {
                case "Update Phone":
                    AnsiConsole.Clear();
                    UpdatePhone(doctorId);
                    break;
                case "Update Password":
                    AnsiConsole.Clear();
                    UpdatePassword(doctorId);
                    break;
                case "Back":
                    AnsiConsole.Clear();
                    return;
                default:
                    AnsiConsole.Clear();
                    AnsiConsole.WriteLine("Invalid choice. Please enter a valid option.");
                    break;
            }
        }

        private void Appointments(int doctorId)
        {
            var appointmentChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(3)
                    .AddChoices(new[] { "Create Appointment", "Delete Appointment", "View Appointments", "Back" }));

            switch (appointmentChoice)
            {
                case "Create Appointment":
                    AnsiConsole.Clear();
                    CreateAppointment(doctorId);
                    break;
                case "Delete Appointment":
                    AnsiConsole.Clear();
                    DeleteAppointment(doctorId);
                    break;
                case "View Appointments":
                    AnsiConsole.Clear();
                    ViewAppointments(doctorId);
                    break;
                case "Back":
                    AnsiConsole.Clear();
                    return;
                default:
                    AnsiConsole.Clear();
                    AnsiConsole.WriteLine("Invalid choice. Please enter a valid option.");
                    break;
            }

            Appointments(doctorId);
        }

        private void CreateAppointment(int doctorId)
        {
            try
            {
                var newAppointment = GetNewAppointmentInput(doctorId);
                var result = _doctorService.CreateAppointmentAsync(newAppointment).Result;

                AnsiConsole.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteLine($"Something went wrong - > {ex.Message}");

                AnsiConsole.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        private void DeleteAppointment(int doctorId)
        {
            try
            {
                var appointmentId = AnsiConsole.Ask<int>("Enter the ID of the Appointment to delete: ");
                var result = _doctorService.DeleteAppointmentAsync(appointmentId).Result;

                AnsiConsole.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteLine($"Something went wrong - > {ex.Message}");

                AnsiConsole.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        private void ViewAppointments(int doctorId)
        {
            try
            {
                var appointments = _doctorService.GetAppointmentsAsync(doctorId).Result;

                if (appointments.Any())
                {
                    var table = new Table().RoundedBorder();
                    table.AddColumn("Appointment ID").AddColumn("Patient ID").AddColumn("Date").AddColumn("Notes");

                    foreach (var appointment in appointments)
                    {
                        table.AddRow(appointment.Id.ToString(), appointment.PatientId.ToString(), appointment.Date.ToString(), appointment.Notes);
                    }

                    AnsiConsole.Write(table);
                }
                else
                {
                    AnsiConsole.WriteLine("No appointments available.");
                }

                AnsiConsole.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteLine($"Something went wrong - > {ex.Message}");

                AnsiConsole.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        private AppointmentCreationModel GetNewAppointmentInput(int doctorId)
        {
            try
            {
                var patientId = AnsiConsole.Ask<int>("Enter the Patient ID: ");
                var patient = _patientService.GetPatientByIdAsync(patientId).Result;
                var date = AnsiConsole.Ask<DateTime>("Enter the appointment date and time: ");
                var notes = AnsiConsole.Ask<string>("Enter any notes for the appointment: ");

                return new AppointmentCreationModel
                {
                    DoctorId = doctorId,
                    PatientId = patientId,
                    Date = date,
                    Notes = notes
                };
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteLine($"Something went wrong - > {ex.Message}");
                return null;
            }
        }

        private void UpdatePhone(int doctorId)
        {
            try
            {
                var phone = GetPhoneInput("Enter new Phone number: ");
                var result = _doctorService.UpdateDoctorAsync(doctorId, new DoctorUpdateModel { PhoneNumber = phone }).Result;
                AnsiConsole.WriteLine("Phone number updated successfully!");

                AnsiConsole.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteLine($"Something went wrong - > {ex.Message}");

                AnsiConsole.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        private void UpdatePassword(int doctorId)
        {
            try
            {
                var newPassword = GetValidPassword("Enter new password: ");
                var doctor = _doctorService.GetDoctorByIdAsync(doctorId).Result;
                if (!VerifyPassword(doctor.Password, newPassword))
                {
                    Console.WriteLine("Incorrect password.");
                    return;
                }
                var result = _doctorService.UpdatePasswordAsync(doctorId, Hashing(newPassword)).Result;
                AnsiConsole.WriteLine("Password updated successfully!");

                AnsiConsole.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteLine($"Something went wrong - > {ex.Message}");

                AnsiConsole.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        private void DisplayDoctorDetails(DoctorViewModel doctor)
        {
            try
            {
                AnsiConsole.WriteLine("Doctor Details");
                var table = new Table();
                table.AddColumn("Property").AddColumn("Value");
                table.AddRow("ID", doctor.Id.ToString());
                table.AddRow("First Name", doctor.FirstName);
                table.AddRow("Last Name", doctor.LastName);
                table.AddRow("Phone Number", doctor.PhoneNumber);
                table.AddRow("Salary", doctor.Salary.ToString());
                table.AddRow("Work Start Time", doctor.WorkStartTime.ToString());
                table.AddRow("Work End Time", doctor.WorkEndTime.ToString());
                table.AddRow("Working Days", doctor.WorkingDays.ToString());
                AnsiConsole.Write(table);

                AnsiConsole.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteLine($"Something went wrong - > {ex.Message}");

                AnsiConsole.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        private string GetValidPassword(string promptMessage)
        {
            string password;
            do
            {
                password = AnsiConsole.Prompt(
                    new TextPrompt<string>($"[green]{promptMessage}[/]")
                        .PromptStyle("red")
                        .Secret()).Trim();

                if (password.Length < 4)
                {
                    AnsiConsole.MarkupLine("[red1]Password must have at least 4 characters.[/]");
                }
            } while (password.Length < 4);

            return password;
        }

        private string Hashing(string password)
        {
            using (SHA256 hash = SHA256.Create())
            {
                byte[] hashedBytes = hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        private void MedicalRecording(int doctorId)
        {
            var medicalRecordChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(5)
                    .AddChoices(new[] { "Add Medical Record", "View Medical Records", "Update Medical Record", "Delete Medical Record", "Back" }));

            switch (medicalRecordChoice)
            {
                case "Add Medical Record":
                    AnsiConsole.Clear();
                    AddMedicalRecord(doctorId);
                    break;
                case "View Medical Records":
                    AnsiConsole.Clear();
                    ViewMedicalRecords();
                    break;
                case "Update Medical Record":
                    AnsiConsole.Clear();
                    UpdateMedicalRecord();
                    break;
                case "Delete Medical Record":
                    AnsiConsole.Clear();
                    DeleteMedicalRecord();
                    break;
                case "Back":
                    AnsiConsole.Clear();
                    return;
                default:
                    AnsiConsole.Clear();
                    AnsiConsole.WriteLine("Invalid choice. Please enter a valid option.");
                    break;
            }

            MedicalRecording(doctorId);
        }

        private void AddMedicalRecord(int doctorId)
        {
            try
            {
                var newMedicalRecord = GetNewMedicalRecordInput(doctorId);

                var result = _medicalRecordService.AddMedicalRecordAsync(newMedicalRecord).Result;

                AnsiConsole.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteLine($"Something went wrong - > {ex.Message}");

                AnsiConsole.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        private void ViewMedicalRecords()
        {
            try
            {
                var medicalRecords = _medicalRecordService.GetAllMedicalRecordsAsync().Result;

                DisplayMedicalRecords(medicalRecords);

                AnsiConsole.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteLine($"Something went wrong - > {ex.Message}");

                AnsiConsole.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        private void UpdateMedicalRecord()
        {
            try
            {
                var updateMedicalRecordInput = GetUpdateMedicalRecordInput();

                var result = _medicalRecordService.UpdateMedicalRecordAsync(updateMedicalRecordInput.Id, updateMedicalRecordInput.MapTo<MedicalRecordUpdateModel>()).Result;

                AnsiConsole.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteLine($"Something went wrong - > {ex.Message}");

                AnsiConsole.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        private void DeleteMedicalRecord()
        {
            try
            {
                var medicalRecordId =  AnsiConsole.Ask<int>("Enter the ID of the Medical Record to delete: ");
                var result = _medicalRecordService.RemoveMedicalRecordAsync(medicalRecordId).Result;

                AnsiConsole.WriteLine("Press any key to continue...");                
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteLine($"Something went wrong - > {ex.Message}");

                AnsiConsole.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        private void DisplayMedicalRecords(List<MedicalRecordViewModel> medicalRecords)
        {
            try
            {
                if (medicalRecords.Any())
                {
                    var table = new Table().RoundedBorder();
                    table.AddColumn("ID").AddColumn("Doctor ID").AddColumn("Patient ID").AddColumn("Diagnosis").AddColumn("Medication").AddColumn("Date Prescribed");

                    foreach (var record in medicalRecords)
                    {
                        table.AddRow(record.Id.ToString(), record.DoctorId.ToString(), record.PatientId.ToString(), record.Diagnosis, record.Medication, record.DatePrescribed.ToString());
                    }

                    AnsiConsole.Write(table);
                }
                else
                {
                    AnsiConsole.WriteLine("No medical records available.");
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteLine($"Something went wrong - > {ex.Message}");
            }
        }

        private MedicalRecordViewModel GetUpdateMedicalRecordInput()
        {
            var diagnosis = AnsiConsole.Ask<string>("Enter the updated diagnosis: ");
            var medication = AnsiConsole.Ask<string>("Enter the updated medication: ");

            return new MedicalRecordViewModel
            {
                Diagnosis = diagnosis,
                Medication = medication,
            };
        }

        private MedicalRecordModel GetNewMedicalRecordInput(int doctorrId)
        {
            try
            {
                var doctorId = doctorrId;
                var patientId = AnsiConsole.Ask<int>("Enter the Patient ID: ");
                var patient = _patientService.GetPatientByIdAsync(patientId).Result;
                var diagnosis = AnsiConsole.Ask<string>("Enter the diagnosis: ");
                var medication = AnsiConsole.Ask<string>("Enter the medication: ");
                var datePrescribed = DateTime.UtcNow;

                return new MedicalRecordModel
                {
                    DoctorId = doctorId,
                    PatientId = patientId,
                    Diagnosis = diagnosis,
                    Medication = medication,
                    DatePrescribed = datePrescribed,
                };
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteLine($"Something went wrong - > {ex.Message}");
                return null;
            }
        }


        private bool VerifyPassword(string actualHashedPassword, string enteredPassword)
        {
            string enteredHashedPassword = Hashing(enteredPassword);
            return actualHashedPassword == enteredHashedPassword;
        }

        private string GetPhoneInput(string text)
        {
            Console.Write(text);
            string input = Console.ReadLine().Trim();
            while (!Regex.IsMatch(input, @"^(\+998|998|0)([1-9]{1}[0-9]{8})$"))
            {
                Console.WriteLine(text);
                input = Console.ReadLine();
            }
            return input;
        }
    }
}
