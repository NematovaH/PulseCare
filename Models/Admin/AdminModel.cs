using PulseCare.Models.Commons;

namespace PulseCare.Models.Admin;

public class AdminModel : Auditable
{
    public string Password { get; set; }
}