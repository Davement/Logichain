using System.ComponentModel.DataAnnotations;

namespace Dtos;

public class UserEditDto
{
    public string? Id { get; set; }
    [Required] public string UserName { get; set; }
    public string? Password { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
}