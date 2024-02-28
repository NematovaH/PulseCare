using PulseCare.Enums;
using PulseCare.Extensions;
using PulseCare.Models.Accountant;
using PulseCare.Models.Doctor;
using PulseCare.Models.Nurse;
using PulseCare.Models.Patient;
using PulseCare.Services;
using Spectre.Console;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace PulseCare.Menus;

public class AdminMenu
{
    private readonly NurseService nurseService;
    private readonly DoctorService doctorService;
    private readonly PatientService patientService;
    private readonly AccountantService accountantService;
    public AdminMenu(AccountantService accountantService, NurseService nurseService, PatientService patientService, DoctorService doctorService)
    {
        this.accountantService = accountantService;
        this.nurseService = nurseService;
        this.doctorService = doctorService;
        this.patientService = patientService;
    }
    public void Run()
    {
        while (true)
        {
            AnsiConsole.Clear();
            var rule = new Rule("\n[red]Admin Menu[/]");
            AnsiConsole.Write(rule); var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(5)
                    .AddChoices(new[] { "Nurse Management", "Doctor Management", "Accountant Management", "Patient Management", "Exit" }));

            switch (choice)
            {
                case "Nurse Management":
                    AnsiConsole.Clear();
                    NurseManagement();
                    break;
                case "Doctor Management":
                    AnsiConsole.Clear();
                    DoctorManagement();
                    break;
                case "Accountant Management":
                    AnsiConsole.Clear();
                    AccountantManagement();
                    break;
                case "Patient Management":
                    AnsiConsole.Clear();
                    PatientManagement();
                    break;
                case "Exit":
                    AnsiConsole.Clear();
                    AnsiConsole.WriteLine("Exiting Admin Menu.");
                    return;
                default:
                    AnsiConsole.Clear();
                    AnsiConsole.WriteLine("Invalid choice. Please enter a valid option.");
                    break;
            }
        }
    }

    #region NURSE
    private void NurseManagement()
    {
        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.WriteLine("Nurse Management");
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(4)
                    .AddChoices(new[] { "View All Nurses", "Create Nurse", "Delete Nurse", "Back" }));

            switch (choice)
            {
                case "View All Nurses":
                    AnsiConsole.Clear();
                    ViewAllNurses();
                    break;
                case "Create Nurse":
                    AnsiConsole.Clear();
                    CreateNurse();
                    break;
                case "Delete Nurse":
                    AnsiConsole.Clear();
                    DeleteNurse();
                    break;
                case "Assign Patient":
                    AnsiConsole.Clear();
                    AssignPatient();
                    break;
                case "Unassign Patient":
                    AnsiConsole.Clear();
                    UnassignPatient();
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
    }

    private void AssignPatient()
    {
        AnsiConsole.WriteLine("Assigning Patient to Nurse:");

        var nurseId = AnsiConsole.Ask<int>("Enter Nurse ID: ");
        var patientId = AnsiConsole.Ask<int>("Enter Patient ID: ");

        try
        {
            var result = nurseService.AssignPatientAsync(nurseId, patientId).Result;
            AnsiConsole.WriteLine("Patient assigned to nurse successfully.");

        }
        catch (Exception ex)
        {
            AnsiConsole.WriteLine($"Error: {ex.Message}");
        }

        AnsiConsole.WriteLine();
        AnsiConsole.Markup("[yellow]Press Enter to continue...[/]");
        Console.ReadLine();
    }

    private void UnassignPatient()
    {
        AnsiConsole.WriteLine("Unassigning Patient from Nurse:");

        var nurseId = AnsiConsole.Ask<int>("Enter Nurse ID: ");

        try
        {
            var result = nurseService.UnassignPatientAsync(nurseId).Result;
            AnsiConsole.WriteLine("Patient unassigned from nurse successfully.");
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteLine($"Error: {ex.Message}");
        }

        AnsiConsole.WriteLine();
        AnsiConsole.Markup("[yellow]Press Enter to continue...[/]");
        Console.ReadLine();
    }

    private void ViewAllNurses()
    {
        try
        {
            var nurses = nurseService.GetAllNursesAsync().Result;
            AnsiConsole.WriteLine("All Nurses");
            if (nurses.Any())
            {
                var table = new Table().Border(TableBorder.Rounded)
                    .AddColumn("ID")
                    .AddColumn("First Name")
                    .AddColumn("Last Name")
                    .AddColumn("Phone Number")
                    .AddColumn("Salary")
                    .AddColumn("Assigned Doctor ID")
                    .AddColumn("Assigned Patient ID");

                foreach (var nurse in nurses)
                {
                    table.AddRow(
                        nurse.Id.ToString(),
                        nurse.FirstName,
                        nurse.LastName,
                        nurse.PhoneNumber,
                        nurse.Salary.ToString(),
                        nurse.AssignedDoctorId.ToString(),
                        nurse.AssignedPatientId.ToString());
                }

                AnsiConsole.Write(table);
            }
            else
            {
                AnsiConsole.WriteLine("No nurses available.");
            }

            AnsiConsole.WriteLine();
            AnsiConsole.Markup("[yellow]Press Enter to continue...[/]");
            Console.ReadLine();
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteLine($"Something went wrong - > {ex.Message}");
        }
    }

    private void CreateNurse()
    {
        try
        {
            AnsiConsole.WriteLine("Creating a Nurse:\n");

            var FirstName = AnsiConsole.Ask<string>("Enter First Name: ");
            var LastName = AnsiConsole.Ask<string>("Enter Last Name: ");
            var phone = GetPhoneInput("Enter Phone number: ");
            var password = GetValidPassword("Enter password: ");
            var AssignedDoctorId = AnsiConsole.Ask<int>("Enter Assigned Doctor ID: ");
            var doctor = doctorService.GetDoctorByIdAsync(AssignedDoctorId).Result;

            var newNurse = new NurseCreationModel();
            newNurse.FirstName = FirstName;
            newNurse.LastName = LastName;
            newNurse.PhoneNumber = phone;
            newNurse.Password = Hashing(password);
            newNurse.AssignedDoctorId = AssignedDoctorId;

            var createdNurse = nurseService.CreateNurseAsync(newNurse).Result;

            AnsiConsole.WriteLine($"Nurse created successfully. ID: {createdNurse.Id}");
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteLine($"Error creating nurse: {ex.Message}");
        }

        AnsiConsole.WriteLine();
        AnsiConsole.Markup("[yellow]Press Enter to continue...[/]");
        Console.ReadLine();
    }

    private void DeleteNurse()
    {
        AnsiConsole.WriteLine("Delete Nurse");
        var nurseId = AnsiConsole.Ask<int>("Enter the ID of the nurse to delete: ");

        try
        {
            var result = nurseService.DeleteNurseAsync(nurseId).Result;
            AnsiConsole.WriteLine("Nurse deleted successfully.");
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteLine($"Error: {ex.Message}");
        }

        AnsiConsole.WriteLine();
        AnsiConsole.Markup("[yellow]Press Enter to continue...[/]");
        Console.ReadLine();
    }

    #endregion

    #region DOCTOR
    private void DoctorManagement()
    {
        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.WriteLine("Doctor Management");
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(4)
                    .AddChoices(new[] { "View All Doctors", "Create Doctor", "Delete Doctor", "Back" }));

            switch (choice)
            {
                case "View All Doctors":
                    AnsiConsole.Clear();
                    ViewAllDoctors();
                    break;
                case "Create Doctor":
                    AnsiConsole.Clear();
                    CreateDoctor();
                    break;
                case "Delete Doctor":
                    AnsiConsole.Clear();
                    DeleteDoctor();
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
    }

    private void ViewAllDoctors()
    {
        var doctors = doctorService.GetAllDoctorsAsync().Result;
        AnsiConsole.WriteLine("All Doctors");
        if (doctors.Any())
        {
            var table = new Table().Border(TableBorder.Rounded)
                .AddColumn("ID")
                .AddColumn("First Name")
                .AddColumn("Last Name")
                .AddColumn("Phone Number")
                .AddColumn("Salary")
                .AddColumn("Specialization")
                .AddColumn("Work Start Time")
                .AddColumn("Work End Time")
                .AddColumn("Working Days");

            foreach (var doctor in doctors)
            {
                table.AddRow(
                    doctor.Id.ToString(),
                    doctor.FirstName,
                    doctor.LastName,
                    doctor.PhoneNumber,
                    doctor.Salary.ToString(),
                    doctor.Specialization.ToString(),
                    doctor.WorkStartTime.ToString("hh\\:mm"),
                    doctor.WorkEndTime.ToString("hh\\:mm"),
                    string.Join(", ", doctor.WorkingDays));
            }

            AnsiConsole.Write(table);
        }
        else
        {
            AnsiConsole.WriteLine("No doctors available.");
        }

        AnsiConsole.WriteLine();
        AnsiConsole.Markup("[yellow]Press Enter to continue...[/]");
        Console.ReadLine();
    }

    private void CreateDoctor()
    {
        AnsiConsole.WriteLine("Creating a Doctor:");
        var FirstName = AnsiConsole.Ask<string>("Enter First Name: ");
        var LastName = AnsiConsole.Ask<string>("Enter Last Name: ");
        var PhoneNumber = GetPhoneInput("Enter Phone number: ");
        var Specialization = ChooseSpecialization();
        var WorkStartTime = GetWorkStartTime();
        var WorkEndTime = GetWorkEndTime(WorkStartTime);
        var Password = GetValidPassword("Enter Password: ");
        var WorkingDays = ChooseWorkingDays();
        var newDoctor = new DoctorCreationModel
        {
            WorkStartTime = WorkStartTime,
            WorkEndTime = WorkEndTime,
            Password = Hashing(Password),
            FirstName = FirstName,
            LastName = LastName,
            PhoneNumber = PhoneNumber,
            Specialization = Specialization,
            WorkingDays = WorkingDays
        };
        try
        {
            var createdDoctor = doctorService.CreateDoctorAsync(newDoctor).Result;
            AnsiConsole.WriteLine($"Doctor created successfully. ID: {createdDoctor.Id}");
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteLine($"Error creating doctor: {ex.Message}");
        }

        AnsiConsole.WriteLine();
        AnsiConsole.Markup("[yellow]Press Enter to continue...[/]");
        Console.ReadLine();
    }

    private TimeSpan GetWorkStartTime()
    {
        return AnsiConsole.Ask<TimeSpan>("Enter Work Start Time (hh:mm): ");
    }

    private TimeSpan GetWorkEndTime(TimeSpan startTime)
    {
        TimeSpan endTime;
        do
        {
            endTime = AnsiConsole.Ask<TimeSpan>("Enter Work End Time (hh:mm): ");
            if (endTime <= startTime)
            {
                AnsiConsole.WriteLine("Work end time must be later than work start time. Please enter valid time.");
            }

        } while (endTime <= startTime);

        return endTime;
    }

    private DoctorSpecialization ChooseSpecialization()
    {
        var specializationChoices = Enum.GetNames(typeof(DoctorSpecialization));
        var specializationPrompt = new SelectionPrompt<string>()
            .Title("Select Specialization:")
            .PageSize(5)
            .AddChoices(specializationChoices);

        var selectedSpecialization = AnsiConsole.Prompt(specializationPrompt);
        return Enum.Parse<DoctorSpecialization>(selectedSpecialization);
    }

    private WorkDays ChooseWorkingDays()
    {
        var workingDaysChoices = Enum.GetNames(typeof(WorkDays));
        var workingDaysPrompt = new MultiSelectionPrompt<string>()
            .Title("Select Working Days:")
            .PageSize(7)
            .AddChoices(workingDaysChoices);

        var selectedWorkingDays = AnsiConsole.Prompt(workingDaysPrompt);
        return selectedWorkingDays.Select(w => Enum.Parse<WorkDays>(w)).Aggregate((days, day) => days | day);
    }

    private void DeleteDoctor()
    {
        AnsiConsole.WriteLine("Delete Doctor");
        var doctorId = AnsiConsole.Ask<int>("Enter the ID of the doctor to delete: ");

        try
        {
            var result = doctorService.DeleteDoctorAsync(doctorId).Result;
            AnsiConsole.WriteLine("Doctor deleted successfully.");
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteLine($"Error: {ex.Message}");
        }

        AnsiConsole.WriteLine();
        AnsiConsole.Markup("[yellow]Press Enter to continue...[/]");
        Console.ReadLine();
    }

    #endregion

    #region ACCOUNTANT
    private void AccountantManagement()
    {
        while (true)
        {

            AnsiConsole.Clear();
            AnsiConsole.WriteLine("Accountant Management");
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(4)
                    .AddChoices(new[] { "View All Accountants", "Create Accountant", "Delete Accountant", "Back" }));

            switch (choice)
            {
                case "View All Accountants":
                    AnsiConsole.Clear();
                    ViewAllAccountants();
                    break;
                case "Create Accountant":
                    AnsiConsole.Clear();
                    CreateAccountant();
                    break;
                case "Delete Accountant":
                    AnsiConsole.Clear();
                    DeleteAccountant();
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
    }

    private void ViewAllAccountants()
    {
        var accountants = accountantService.GetAllAccountantsAsync().Result;
        AnsiConsole.WriteLine("All Accountants");
        if (accountants.Any())
        {
            var table = new Table().Border(TableBorder.Rounded)
                .AddColumn("ID")
                .AddColumn("First Name")
                .AddColumn("Last Name")
                .AddColumn("Phone Number")
                .AddColumn("Salary");

            foreach (var accountant in accountants)
            {
                table.AddRow(
                    accountant.Id.ToString(),
                    accountant.FirstName,
                    accountant.LastName,
                    accountant.PhoneNumber,
                    accountant.Salary.ToString());
            }

            AnsiConsole.Write(table);
        }
        else
        {
            AnsiConsole.WriteLine("No accountants available.");
        }

        AnsiConsole.WriteLine();
        AnsiConsole.Markup("[yellow]Press Enter to continue...[/]");
        Console.ReadLine();
    }

    private void CreateAccountant()
    {
        var newAccountant = new AccountantCreationModel
        {
            FirstName = AnsiConsole.Ask<string>("Enter First Name: "),
            LastName = AnsiConsole.Ask<string>("Enter Last Name: "),
            PhoneNumber = GetPhoneInput("Enter Phone number: "),
            Password = GetValidPassword("Enter Password: "),
        };
        newAccountant.Password = Hashing(newAccountant.Password);
        try
        {
            var createdAccountant = accountantService.CreateAccountantAsync(newAccountant).Result;
            AnsiConsole.WriteLine($"Accountant created successfully. ID: {createdAccountant.Id}");
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteLine($"Error creating accountant: {ex.Message}");
        }

        AnsiConsole.WriteLine();
        AnsiConsole.Markup("[yellow]Press Enter to continue...[/]");
        Console.ReadLine();
    }

    private void DeleteAccountant()
    {
        AnsiConsole.WriteLine("Delete Accountant");
        var accountantId = AnsiConsole.Ask<int>("Enter the ID of the accountant to delete: ");

        try
        {
            var result = accountantService.DeleteAccountantAsync(accountantId).Result;
            if (result)
            {
                AnsiConsole.WriteLine("Accountant deleted successfully.");
            }
            else
            {
                AnsiConsole.WriteLine($"Unable to delete accountant with ID {accountantId}.");
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteLine($"Error: {ex.Message}");
        }

        AnsiConsole.WriteLine();
        AnsiConsole.Markup("[yellow]Press Enter to continue...[/]");
        Console.ReadLine();
    }

    #endregion

    #region PATIENT
    private void PatientManagement()
    {
        while (true)
        {

            AnsiConsole.Clear();
            AnsiConsole.WriteLine("Patient Management");
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(4)
                    .AddChoices(new[] { "View All Patients", "Create Patient", "Delete Patient", "Back" }));

            switch (choice)
            {
                case "View All Patients":
                    AnsiConsole.Clear();
                    ViewAllPatients();
                    break;
                case "Create Patient":
                    AnsiConsole.Clear();
                    CreatePatient();
                    break;
                case "Delete Patient":
                    AnsiConsole.Clear();
                    DeletePatient();
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
    }

    private void ViewAllPatients()
    {
        var patients = patientService.GetAllPatientsAsync().Result;
        AnsiConsole.WriteLine("All Patients");
        if (patients.Any())
        {
            var table = new Table().Border(TableBorder.Rounded)
                .AddColumn("ID")
                .AddColumn("First Name")
                .AddColumn("Last Name")
                .AddColumn("Gender")
                .AddColumn("Date of Birth")
                .AddColumn("Phone Number")
                .AddColumn("Email")
                .AddColumn("Address")
                .AddColumn("Balance")
                .AddColumn("Assigned Doctor ID")
                .AddColumn("Assigned Nurse ID")
                .AddColumn("Status");

            foreach (var patient in patients)
            {
                table.AddRow(
                    patient.Id.ToString(),
                    patient.FirstName,
                    patient.LastName,
                    patient.Gender.ToString(),
                    patient.DateOfBirth.ToString("yyyy-MM-dd"),
                    patient.PhoneNumber,
                    patient.Email,
                    patient.Address,
                    patient.Balance.ToString(),
                    patient.AssignedDoctorId.ToString(),
                    patient.AssignedNurseId.ToString(),
                    patient.Status.ToString());
            }

            AnsiConsole.Write(table);
        }
        else
        {
            AnsiConsole.WriteLine("No patients available.");
        }

        AnsiConsole.WriteLine();
        AnsiConsole.Markup("[yellow]Press Enter to continue...[/]");
        Console.ReadLine();
    }

    private void CreatePatient()
    {
        var newPatient = new PatientModel
        {
            FirstName = AnsiConsole.Ask<string>("Enter First Name: "),
            LastName = AnsiConsole.Ask<string>("Enter Last Name: "),
            PhoneNumber = GetPhoneInput("Enter Phone number: "),
            Address = AnsiConsole.Ask<string>("Enter address: "),
            AssignedDoctorId = AnsiConsole.Ask<int>("Enter doctor Id: "),
            AssignedNurseId = AnsiConsole.Ask<int>("Enter nurse Id: "),
            Balance = AnsiConsole.Ask<decimal>("Enter your balance: "),
            DateOfBirth = AnsiConsole.Ask<DateOnly>("Enter date of birth: "),
            Email = GetValidEmail("Enter your email: "),
            Status = PatientStatus.Unhealthy,
            Password = GetValidPassword("Enter Password: "),
        };
        newPatient.Password = Hashing(newPatient.Password);
        var genderPrompt = new SelectionPrompt<string>()
            .Title("Enter gender: ")
            .PageSize(3)
            .AddChoices("Male", "Female");

        var selectedGender = AnsiConsole.Prompt(genderPrompt);

        newPatient.Gender = selectedGender;
          
        try
        {
            var createdPatient = patientService.CreatePatientAsync(newPatient.MapTo<PatientCreationModel>()).Result;
            var nurse = nurseService.AssignPatientAsync(newPatient.AssignedNurseId, newPatient.Id);
            AnsiConsole.WriteLine($"Patient created successfully. ID: {createdPatient.Id}");
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteLine($"Error creating patient: {ex.Message}");
        }

        AnsiConsole.WriteLine();
        AnsiConsole.Markup("[yellow]Press Enter to continue...[/]");
        Console.ReadLine();
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

    private void DeletePatient()
    {
        AnsiConsole.WriteLine("Delete Patient");
        var patientId = AnsiConsole.Ask<int>("Enter the ID of the patient to delete: ");

        try
        {
            var result = patientService.DeletePatientAsync(patientId).Result;
            if (result)
            {
                AnsiConsole.WriteLine("Patient deleted successfully.");
            }
            else
            {
                AnsiConsole.WriteLine($"Unable to delete patient with ID {patientId}.");
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteLine($"Error: {ex.Message}");
        }

        AnsiConsole.WriteLine();
        AnsiConsole.Markup("[yellow]Press Enter to continue...[/]");
        Console.ReadLine();
    }

    #endregion

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
