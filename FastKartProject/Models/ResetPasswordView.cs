using System.ComponentModel.DataAnnotations;

namespace FastKartProject.Models;

public class ResetPasswordView
{
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [DataType(DataType.Password), Compare(nameof(Password))]
    public string ConfirmPassword { get; set; }
    //public string? Email { get; set; }
    //public string? ResetToken { get; set; }

}
