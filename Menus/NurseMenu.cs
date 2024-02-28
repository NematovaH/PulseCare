using PulseCare.Models.Nurse;
using PulseCare.Services;
using Spectre.Console;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace PulseCare.Menu
{
    public class NurseMenu
    {
        private readonly NurseService _nurseService;
        private readonly DoctorService _doctorService;
        private readonly PatientService patientService;

        public NurseMenu(NurseService nurseService, DoctorService doctorService, PatientService patientService)
        {
            _nurseService = nurseService;
            _doctorService = doctorService;
            this.patientService = patientService;
        }

        public void Login(int nurseId)
        {
            var rule = new Rule("\n[red]Nurse Menu[/]");
            AnsiConsole.Write(rule);

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(5)
                    .AddChoices(new[] { "My Profile", "Update Details", "My Patients", "Salary Notification Box", "Exit" }));

            switch (choice)
            {
                case "My Profile":
                    AnsiConsole.Clear();
                    MyProfile(nurseId);
                    break;
                case "Update Details":
                    AnsiConsole.Clear();
                    UpdateDetails(nurseId);
                    break;
                case "My Patients":
                    AnsiConsole.Clear();
                    MyPatients(nurseId);
                    break;
                case "Salary Notification Box":
                    AnsiConsole.Clear();
                    SalaryNotificationBox(nurseId);
                    break;
                case "Exit":
                    AnsiConsole.Clear();
                    AnsiConsole.WriteLine("Exiting Nurse Menu.");
                    return;
                default:
                    AnsiConsole.Clear();
                    AnsiConsole.WriteLine("Invalid choice. Please enter a valid option.");
                    break;
            }

            Login(nurseId); 
        }

        private void MyProfile(int nurseId)
        {
            try
            {

                var nurse = _nurseService.GetNurseByIdAsync(nurseId).Result;
                DisplayNurseDetails(nurse);
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteLine($"Something went wrong - > {ex.Message}");

                AnsiConsole.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        private void UpdateDetails(int nurseId)
        {
            AnsiConsole.Clear();
            var updateChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(3)
                    .AddChoices(new[] { "Update Phone", "Update Password", "Back" }));

            switch (updateChoice)
            {
                case "Update Phone":
                    AnsiConsole.Clear();
                    UpdatePhone(nurseId);
                    break;
                case "Update Password":
                    AnsiConsole.Clear();
                    UpdatePassword(nurseId);
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

        private void MyPatients(int nurseId)
        {
            try
            {
                AnsiConsole.WriteLine("Displaying Assigned Patient Details...");

                var nurse = _nurseService.GetNurseByIdAsync(nurseId).Result;
                if (nurse.AssignedPatientId == 0)
                {
                    AnsiConsole.WriteLine("No patient is available for now");
                    return;
                }

                var patient = patientService.GetPatientByIdAsync(nurse.AssignedPatientId).Result;

                var table = new Table().RoundedBorder();
                table.AddColumn("Property").AddColumn("Value");
                table.AddRow("Patient ID", patient.Id.ToString());
                table.AddRow("First Name", patient.FirstName);
                table.AddRow("Last Name", patient.LastName);
                table.AddRow("Gender", patient.Gender.ToString());
                table.AddRow("Date of Birth", patient.DateOfBirth.ToString());
                table.AddRow("Phone Number", patient.PhoneNumber);
                table.AddRow("Email", patient.Email);
                table.AddRow("Address", patient.Address);
                table.AddRow("Status", patient.Status.ToString());

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

        private void SalaryNotificationBox(int nurseId)
        {
            try
            {
                var messages = _nurseService.ViewMessageBox(nurseId).Result;

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

        private void UpdatePhone(int nurseId)
        {
            try
            {
                var phone = GetPhoneInput("Enter new Phone number: ");
                var result = _nurseService.UpdatePhoneAsync(nurseId, phone).Result;
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

        private void UpdatePassword(int nurseId)
        {
            try
            {
                var newPassword = GetValidPassword("Enter new password: ");
                var nurse = _nurseService.GetNurseByIdAsync(nurseId).Result;
                if (!VerifyPassword(nurse.Password, newPassword))
                {
                    Console.WriteLine("Incorrect password.");
                    return;
                }
                var result = _nurseService.UpdatePasswordAsync(nurseId, Hashing(newPassword)).Result;

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

        private void DisplayNurseDetails(NurseViewModel nurse)
        {
            try
            {
                var doctor = _doctorService.GetDoctorByIdAsync(nurse.AssignedDoctorId).Result;
                AnsiConsole.WriteLine("Nurse Details");
                var table = new Table();
                table.AddColumn("Property").AddColumn("Value");
                table.AddRow("ID", nurse.Id.ToString());
                table.AddRow("First Name", nurse.FirstName);
                table.AddRow("Last Name", nurse.LastName);
                table.AddRow("Phone Number", nurse.PhoneNumber);
                table.AddRow("Salary", nurse.Salary.ToString());
                table.AddRow("Assigned Doctor name", doctor.FirstName + doctor.LastName);
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
