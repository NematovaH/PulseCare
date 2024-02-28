using PulseCare.Models.Patient;
using PulseCare.Services;
using Spectre.Console;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace PulseCare.Menu;

public class PatientMenu
{
    private readonly PatientService _patientService;
    private readonly NurseService _nurseService;
    private readonly HospitalService _hospitalService;

    public PatientMenu(PatientService patientService, NurseService nurseService, HospitalService hospitalService)
    {
        _patientService = patientService;
        _nurseService = nurseService;
        _hospitalService = hospitalService;
    }

    public void Login(int patientId)
    {
        var rule = new Rule("\n[red]Patient Menu[/]");
        AnsiConsole.Write(rule);

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .PageSize(5)
                .AddChoices(new[] { "My Profile", "Update Details", "Appointments", "Payment", "Upgrade Balance", "Exit" }));

        switch (choice)
        {
            case "My Profile":
                AnsiConsole.Clear();
                MyProfile(patientId);
                break;
            case "Update Details":
                AnsiConsole.Clear();
                UpdateDetails(patientId);
                break;
            case "Appointments":
                AnsiConsole.Clear();
                Appointments(patientId);
                break;
            case "Payment":
                AnsiConsole.Clear();
                PayingForAllServices(patientId);
                break;
            case "Upgrade Balance":
                AnsiConsole.Clear();
                UpgradeBalance(patientId);
                break;
            case "Exit":
                AnsiConsole.Clear();
                AnsiConsole.WriteLine("Exiting Patient Menu.");
                return;
            default:
                AnsiConsole.Clear();
                AnsiConsole.WriteLine("Invalid choice. Please enter a valid option.");
                break;
        }

        Login(patientId);
    }

    private void PayingForAllServices(int patientId)
    {
        try
        {
            var res = _patientService.GetPatientByIdAsync(patientId).Result;
            var result = _patientService.DegradeBalanceAsync(patientId, 1234).Result;
            var hos = _hospitalService.UpgradeBalance(1, 1234);
            AnsiConsole.WriteLine("Payment is done successfully");

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

    private void MyProfile(int patientId)
    {
        try
        {
            var patient = _patientService.GetPatientByIdAsync(patientId).Result;
            DisplayPatientDetails(patient);

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

    private void UpdateDetails(int patientId)
    {
        AnsiConsole.Clear();
        var updateChoice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .PageSize(4)
                .AddChoices(new[] { "Update Password", "Update Patient Info", "Back" }));

        switch (updateChoice)
        {
            case "Update Password":
                AnsiConsole.Clear();
                UpdatePassword(patientId);
                break;
            case "Update Patient Info":
                AnsiConsole.Clear();
                UpdatePatientInfo(patientId);
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

    private void Appointments(int patientId)
    {
        try
        {
            var appointments = _patientService.GetAppointmentsAsync(patientId).Result;

            if (appointments.Any())
            {
                var table = new Table().RoundedBorder();
                table.AddColumn("Appointment ID").AddColumn("Doctor ID").AddColumn("Date").AddColumn("Notes");

                foreach (var appointment in appointments)
                {
                    table.AddRow(appointment.Id.ToString(), appointment.DoctorId.ToString(), appointment.Date.ToString(), appointment.Notes);
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

    private void UpgradeBalance(int patientId)
    {
        try
        {
            decimal amount;

            do
            {
                amount = AnsiConsole.Ask<decimal>("Enter amount to upgrade balance: ");

                if (amount <= 0)
                {
                    AnsiConsole.WriteLine("Amount must be greater than 0. Please enter a valid amount.");
                }

            } while (amount <= 0);

            var result = _patientService.UpgradeBalanceAsync(patientId, amount).Result;
            AnsiConsole.WriteLine("Balance upgraded successfully!");

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

    private void UpdatePassword(int patientId)
    {
        try
        {
            var newPassword = GetValidPassword("Enter new password: ");
            var nurse = _patientService.GetPatientByIdAsync(patientId).Result;
            var result = _patientService.UpdatePasswordAsync(patientId, Hashing(newPassword)).Result;

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

    private void UpdatePatientInfo(int patientId)
    {
        try
        {
            var updatedPatient = GetUpdatedPatientInfo();
            var result = _patientService.UpdatePatientAsync(patientId, updatedPatient).Result;
            AnsiConsole.WriteLine("Patient information updated successfully!");

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

    private void DisplayPatientDetails(PatientViewModel patient)
    {
        try
        {
            AnsiConsole.WriteLine("Patient Details");
            var table = new Table();
            table.AddColumn("Property").AddColumn("Value");
            table.AddRow("ID", patient.Id.ToString());
            table.AddRow("First Name", patient.FirstName);
            table.AddRow("Last Name", patient.LastName);
            table.AddRow("Phone Number", patient.PhoneNumber);
            table.AddRow("Email", patient.Email);
            table.AddRow("Address", patient.Address);
            table.AddRow("Balance", patient.Balance.ToString());
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

    private PatientUpdateModel GetUpdatedPatientInfo()
    {
        var updatedPatient = new PatientUpdateModel();

        updatedPatient.PhoneNumber = GetValidPhoneNumber("Enter new phone number: ");
        updatedPatient.Email = GetValidEmail("Enter new email: ");
        updatedPatient.Address = AnsiConsole.Ask<string>("Enter new address: ");

        return updatedPatient;
    }

    private string GetValidPhoneNumber(string promptMessage)
    {
        string phoneNumber;
        do
        {
            phoneNumber = AnsiConsole.Prompt(
                new TextPrompt<string>($"[green]{promptMessage}[/]")
                    .PromptStyle("red")
                    .Validate(phone => Regex.IsMatch(phone, @"^(\+998|998|0)([1-9]{1}[0-9]{8})$"), "Invalid phone number."))
                .Trim();
        } while (!Regex.IsMatch(phoneNumber, @"^(\+998|998|0)([1-9]{1}[0-9]{8})$"));

        return phoneNumber;
    }

    private string GetValidEmail(string promptMessage)
    {
        string email;
        do
        {
            email = AnsiConsole.Prompt(
                new TextPrompt<string>($"[green]{promptMessage}[/]")
                    .PromptStyle("red")
                    .Validate(email => Regex.IsMatch(email, @"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$"), "Invalid email address."))
                .Trim();
        } while (!Regex.IsMatch(email, @"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$"));

        return email;
    }

    private string Hashing(string password)
    {
        using (SHA256 hash = SHA256.Create())
        {
            byte[] hashedBytes = hash.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }
    }
}
