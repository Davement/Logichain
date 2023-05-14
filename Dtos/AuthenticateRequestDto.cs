using System.ComponentModel.DataAnnotations;
#pragma warning disable CS8618

namespace Dtos;

public class AuthenticateRequestDto
{
    [Required] public string UserName { get; set; }
    [Required] public string Password { get; set; }
}