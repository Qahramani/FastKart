using System.ComponentModel.DataAnnotations;

namespace FastKartProject.Models;

public class LoginViewModel
{
    public string Username { get; set; }
    [DataType(DataType.Password)]
    public string Password { get; set; }
}
