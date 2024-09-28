using System.ComponentModel.DataAnnotations;

namespace FastKartProject.Models;

public class ForgotPasswordViewModel
{
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }

}
