using Dtos;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.User;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Logichain.Controllers.v1;

[Route("api/v1/users")]
[Produces("application/json")]
public class UserV1Controller
{
    private readonly IUserService _userService;

    public UserV1Controller(IUserService userService)
    {
        _userService = userService;
    }
    
    [HttpGet]
    public async Task<UserInfoDto> GetUser(string id)
    {
        return await _userService.GetUser(id);
    }
    
    [HttpGet("list")]
    public List<UserInfoDto> GetUsers()
    {
        return _userService.GetUsers();
    }

    [HttpPost("token")]
    public async Task<SignInResult> Authenticate(AuthenticateRequestDto authenticateRequestDto)
    {
        return await _userService.Authenticate(authenticateRequestDto);
    }

    [HttpPost]
    public async Task CreateUser(UserEditDto userEditDto)
    {
        await _userService.CreateUser(userEditDto);
    }
    
    [HttpPut]
    public async Task UpdateUser(UserEditDto userEditDto)
    {
        await _userService.UpdateUser(userEditDto);
    }
}