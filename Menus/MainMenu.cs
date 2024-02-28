using PulseCare.Menu;
using PulseCare.Models.Hospital;
using PulseCare.Services;
using Spectre.Console;
using System.Security.Cryptography;
using System.Text;

namespace PulseCare.Menus;

public class MainMenu
{
    private readonly AccountantService accountantService;
    private readonly AdminService adminService;
    private readonly DoctorService doctorService;
    private readonly NurseService nurseService;
    private readonly PatientService patientService;

    private readonly HospitalService hospitalService;
    private readonly MedicalRecordService mediicalRecordService;

    private readonly AdminMenu adminMenu;
    private readonly NurseMenu nurseMenu;
    private readonly DoctorMenu doctorMenu;
    private readonly AccountantMenu accountantMenu;
    private readonly PatientMenu patientMenu;
    private readonly HospitalMenu hospitalMenu;
    public MainMenu()
    {
        accountantService = new AccountantService();
        adminService = new AdminService();
        doctorService = new DoctorService();
        patientService = new PatientService();
        nurseService = new NurseService();
        hospitalService = new HospitalService();
        mediicalRecordService = new MedicalRecordService();

        adminMenu = new AdminMenu(accountantService, nurseService, patientService, doctorService);
        nurseMenu = new NurseMenu(nurseService, doctorService, patientService);
        doctorMenu = new DoctorMenu(doctorService, mediicalRecordService, patientService);
        patientMenu = new PatientMenu(patientService, nurseService, hospitalService);
        hospitalMenu = new HospitalMenu(hospitalService, doctorService, nurseService, patientService);
        accountantMenu = new AccountantMenu(accountantService);
    }
    public void Run()
    {
        Initialize();
        while (true)
        {
            AnsiConsole.Clear();
            var rule = new Rule("\n[red]Main Menu[/]");
            AnsiConsole.Write(rule); var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(10)
                    .AddChoices(new[] { "Main Manager", "Patient", "Doctor", "Nurse", "Accountant", "HospitalInfo", "Exit" }));

            switch (choice)
            {
                case "Main Manager":
                    AnsiConsole.Clear();
                    try
                    {
                        string password;
                        do
                        {
                            password = AnsiConsole.Prompt(
                                new TextPrompt<string>("Enter[green] password[/]?")
                                    .PromptStyle("red")
                                    .Secret()).Trim();

                            if (password.Length < 4)
                            {
                                AnsiConsole.MarkupLine("[red1]Password must have at least 4 characters.[/]");
                            }
                        } while (password.Length < 4);
                        if (!VerifyPassword(Hashing("0000"), password))
                        {
                            Console.WriteLine("Incorrect password.");
                            AnsiConsole.WriteLine("enter to continue");
                            Console.ReadKey();
                            break;
                        }
                        adminMenu.Run();
                    }
                    catch (Exception ex)
                    {
                        AnsiConsole.WriteLine($"Login went wrong - > {ex.Message}");
                    }
                    break;
                case "Patient":
                    AnsiConsole.Clear();
                    var patientId = AnsiConsole.Ask<int>("Enter your ID (as a patient) : ");
                    try
                    {
                        string password;
                        do
                        {
                            password = AnsiConsole.Prompt(
                                new TextPrompt<string>("Enter[green] password[/]?")
                                    .PromptStyle("red")
                                    .Secret()).Trim();

                            if (password.Length < 4)
                            {
                                AnsiConsole.MarkupLine("[red1]Password must have at least 4 characters.[/]");
                            }
                        } while (password.Length < 4);

                        var patient = patientService.GetPatientByIdAsync(patientId).Result;

                        if (!VerifyPassword(patient.Password, password))
                        {
                            Console.WriteLine("Incorrect password.");
                            AnsiConsole.WriteLine("enter to continue");
                            Console.ReadKey();
                            break;
                        }
                        patientMenu.Login(patientId);
                    }
                    catch (Exception ex)
                    {
                        AnsiConsole.WriteLine($"Login went wrong - > {ex.Message}");
                    }
                    break;

                case "Doctor":
                    AnsiConsole.Clear();
                    var doctorId = AnsiConsole.Ask<int>("Enter your ID (as a doctor) : ");
                    try
                    {
                        string password;
                        do
                        {
                            password = AnsiConsole.Prompt(
                                new TextPrompt<string>("Enter[green] password[/]?")
                                    .PromptStyle("red")
                                    .Secret()).Trim();

                            if (password.Length < 4)
                            {
                                AnsiConsole.MarkupLine("[red1]Password must have at least 4 characters.[/]");
                            }
                        } while (password.Length < 4);
                        var doctor = doctorService.GetDoctorByIdAsync(doctorId).Result;
                        if (!VerifyPassword(doctor.Password, password))
                        {
                            Console.WriteLine("Incorrect password.");
                            AnsiConsole.WriteLine("enter to continue");
                            Console.ReadKey();
                            break;
                        }
                        var res = doctorService.GetDoctorByIdAsync(doctorId).Result;
                        doctorMenu.Login(doctorId);
                    }
                    catch (Exception ex)
                    {
                        AnsiConsole.WriteLine($"Login went wrong - > {ex.Message}");
                    }
                    break;
                case "Nurse":
                    AnsiConsole.Clear();
                    var nurseId = AnsiConsole.Ask<int>("Enter your ID (as a nurse) : ");
                    try
                    {
                        string password;
                        do
                        {
                            password = AnsiConsole.Prompt(
                                new TextPrompt<string>("Enter[green] password[/]?")
                                    .PromptStyle("red")
                                    .Secret()).Trim();

                            if (password.Length < 4)
                            {
                                AnsiConsole.MarkupLine("[red1]Password must have at least 4 characters.[/]");
                            }
                        } while (password.Length < 4);
                        var nurse = nurseService.GetNurseByIdAsync(nurseId).Result;
                        if (!VerifyPassword(nurse.Password, password))
                        {
                            Console.WriteLine("Incorrect password.");
                            AnsiConsole.WriteLine("enter to continue");
                            Console.ReadKey();
                            break;
                        }
                        var res = nurseService.GetNurseByIdAsync(nurseId).Result;
                        nurseMenu.Login(nurseId);
                    }
                    catch (Exception ex)
                    {
                        AnsiConsole.WriteLine($"Login went wrong - > {ex.Message}");
                    }
                    break;
                case "Accountant":
                    AnsiConsole.Clear();
                    var accountantId = AnsiConsole.Ask<int>("Enter your ID (as an accountant) : ");
                    try
                    {
                        string password;
                        do
                        {
                            password = AnsiConsole.Prompt(
                                new TextPrompt<string>("Enter[green] password[/]?")
                                    .PromptStyle("red")
                                    .Secret()).Trim();

                            if (password.Length < 4)
                            {
                                AnsiConsole.MarkupLine("[red1]Password must have at least 4 characters.[/]");
                            }
                        } while (password.Length < 4);
                        var accountant = accountantService.GetAccountantByIdAsync(accountantId).Result;
                        if (!VerifyPassword(accountant.Password, password))
                        {
                            Console.WriteLine("Incorrect password.");
                            AnsiConsole.WriteLine("enter to continue");
                            Console.ReadKey();
                            break;
                        }
                        var res = accountantService.GetAccountantByIdAsync(accountantId).Result; 
                        accountantMenu.Login(accountantId);
                    }
                    catch (Exception ex)
                    {
                        AnsiConsole.WriteLine($"Login went wrong - > {ex.Message}");
                    }
                    break;

                case "HospitalInfo":
                    AnsiConsole.Clear();
                    try
                    {
                        hospitalMenu.Run(1);
                    }
                    catch(Exception ex)
                    {
                        AnsiConsole.WriteLine($"Something went wrong - > {ex.Message}");
                    }
                    break;
                case "Exit":
                    AnsiConsole.Clear();
                    AnsiConsole.WriteLine("Exiting the application.");
                    return;
                default:
                    AnsiConsole.Clear();
                    AnsiConsole.WriteLine("Invalid choice. Please enter a valid option.");
                    break;
            }
        }
    }

    private void Initialize()
    {
        try
        {
            AnsiConsole.Write(new Markup("\t\t\t\t\t\t[bold red]Welcome[/] [red]to[/] [yellow]Pul$eCare![/]\n\n"));
            var hospital = new HospitalCreationModel();
            hospital.Balance = 9999999999;
            hospital.Name = "PulseCARE";
            hospital.RoomsCount = 300;
            var res = hospitalService.CreateHospitalAsync(hospital);
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteLine($"Something went wrong - > {ex.Message}");
        }
    }

    private bool VerifyPassword(string actualHashedPassword, string enteredPassword)
    {
        string enteredHashedPassword = Hashing(enteredPassword);
        return actualHashedPassword == enteredHashedPassword;
    }

    private static string Hashing(string password)
    {
        using (SHA256 hash = SHA256.Create())
        {
            byte[] hashedBytes = hash.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }
    }
}

