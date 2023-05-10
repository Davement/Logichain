using System.ComponentModel.DataAnnotations;

namespace Dtos;

public class AuthenticateRequestDto
{
    [Required] public string UserName { get; set; }
    [Required] public string Password { get; set; }
}