namespace PulseCare.Models.Accountant;

public class AccountantUpdateModel
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
    public decimal Salary { get; set; }
    public List<string> MessageBox { get; set; } = new List<string>();
}