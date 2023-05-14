using System.ComponentModel.DataAnnotations;
#pragma warning disable CS8618

namespace Dtos;

public class UserEditDto
{
    public string? Id { get; set; }
    [Required] public string UserName { get; set; }
    public string? Password { get; set; }
    [Required] public string Email { get; set; }
    public string? PhoneNumber { get; set; }
}