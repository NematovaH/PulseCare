using PulseCare.Models.Accountant;
using PulseCare.Services;
using Spectre.Console;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace PulseCare.Menus;

internal class AccountantMenu
{
    private readonly AccountantService _accountantService;

    public AccountantMenu(AccountantService accountantService)
    {
        _accountantService = accountantService;
    }

    public void Login(int accountantId)
    {
        AnsiConsole.Clear();
        var rule = new Rule("\n[red]Accountant Menu[/]");
        AnsiConsole.Write(rule);
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .PageSize(5)
                .AddChoices(new[] { "My Profile", "Update Details", "Distribute Salary", "Exit" }));

        switch (choice)
        {
            case "My Profile":
                AnsiConsole.Clear();
                ViewAccountantProfile(accountantId);
                break;
            case "Update Details":
                AnsiConsole.Clear();
                UpdateAccountantDetails(accountantId);
                break;
            case "Distribute Salary":
                AnsiConsole.Clear();
                DistributeSalary();
                break;
            case "Exit":
                AnsiConsole.Clear();
                AnsiConsole.WriteLine("Exiting Accountant Menu.");
                break;
            default:
                AnsiConsole.Clear();
                AnsiConsole.WriteLine("Invalid choice. Please enter a valid option.");
                break;
        }
    }

    private void ViewAccountantProfile(int accountantId)
    {
        try
        {
            var accountant = _accountantService.GetAccountantByIdAsync(accountantId).Result;
            DisplayAccountantDetails(accountant);

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

    private void UpdateAccountantDetails(int accountantId)
    {
        AnsiConsole.Clear();
        var updateChoice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .PageSize(3)
                .AddChoices(new[] { "Update Phone", "Update Password", "Back" }));

        switch (updateChoice)
        {
            case "Update Phone":
                AnsiConsole.Clear ();
                UpdatePhone(accountantId);
                break;
            case "Update Password":
                AnsiConsole.Clear();
                UpdatePassword(accountantId);
                break;
            case "Back":
                AnsiConsole.Clear();
                break;
            default:
                AnsiConsole.Clear();
                AnsiConsole.WriteLine("Invalid choice. Please enter a valid option.");
                break;
        }
    }

    private void UpdatePhone(int accountantId)
    {
        try
        {
            var phone = GetPhoneInput("Enter new Phone number: ");
            var result = _accountantService.UpdatePhoneNumberAsync(accountantId, phone).Result;
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

    private void UpdatePassword(int accountantId)
    {
        try
        {
            var newPassword = GetValidPassword("Enter new password: ");
            var nurse = _accountantService.GetAccountantByIdAsync(accountantId).Result;
            if (!VerifyPassword(nurse.Password, newPassword))
            {
                Console.WriteLine("Incorrect password.");
                return;
            }
            var result = _accountantService.UpdatePasswordAsync(accountantId, Hashing(newPassword)).Result;

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

    private void DistributeSalary()
    {
        try
        {
            var result = _accountantService.DistributeSalaryToWorkersAsync().Result;
            AnsiConsole.WriteLine("Salary distributed successfully!");

            AnsiConsole.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteLine($"Error distributing salary: {ex.Message}");

            AnsiConsole.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }

    private void DisplayAccountantDetails(AccountantViewModel accountant)
    {
        AnsiConsole.WriteLine("Accountant Details");
        var table = new Table();
        table.AddColumn("Property").AddColumn("Value");
        table.AddRow("ID", accountant.Id.ToString());
        table.AddRow("First Name", accountant.FirstName);
        table.AddRow("Last Name", accountant.LastName);
        table.AddRow("Phone Number", accountant.PhoneNumber);
        table.AddRow("Salary", accountant.Salary.ToString());
        AnsiConsole.Write(table);
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
}
