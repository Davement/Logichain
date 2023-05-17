using System.Security.Claims;
using Dtos;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Models;

namespace Services.User;

public interface IUserService
{
    Task<IdentityResult> CreateUser(UserEditDto userEditDto);
    Task<UserInfoDto> GetUser(string id);
    List<UserInfoDto> GetUsers();
    Task<IdentityResult> UpdateUser(UserEditDto userEditDto);
    Task<SignInResult> Authenticate(AuthenticateRequestDto authenticateRequestDto);
    Task<IdentityUser> GetCurrentUser();
}

public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserService(ILogger<UserService> logger, UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<UserInfoDto> GetUser(string id)
    {
        var identityUser = await _userManager.FindByIdAsync(id);
        if (identityUser != null)
        {
            return identityUser.Adapt<UserInfoDto>();
        }

        _logger.Log(LogLevel.Error, $"User with id '{id}' not found");
        return new UserInfoDto();
    }

    public List<UserInfoDto> GetUsers()
    {
        var identityUsers = _userManager.Users.ToList();
        return identityUsers.Adapt<List<UserInfoDto>>();
    }

    public async Task<SignInResult> Authenticate(AuthenticateRequestDto authenticateRequestDto)
    {
        var identityUser = await _userManager.FindByNameAsync(authenticateRequestDto.UserName);
        if (identityUser == null)
        {
            _logger.Log(LogLevel.Error, $"User with user name '{authenticateRequestDto.UserName}' not found");
            return SignInResult.Failed;
        }

        return await _signInManager.CheckPasswordSignInAsync(identityUser, authenticateRequestDto.Password, true);
    }

    public async Task<IdentityResult> CreateUser(UserEditDto userEditDto)
    {
        var identityUser = new IdentityUser
        {
            UserName = userEditDto.UserName
        };
        if (userEditDto.Password == null)
        {
            _logger.Log(LogLevel.Error, "Password cannot be empty");
            return IdentityResult.Failed();
        }

        identityUser.PasswordHash = HashPassword(identityUser, userEditDto.Password);

        var result = await _userManager.CreateAsync(identityUser);
        if (result.Succeeded)
        {
            _logger.Log(LogLevel.Information, $"User '{identityUser.UserName}' created");
            return result;
        }

        return result;
    }

    public async Task<IdentityResult> UpdateUser(UserEditDto userEditDto)
    {
        var identityUser = await _userManager.FindByIdAsync(userEditDto.Id!);
        if (identityUser == null)
        {
            _logger.Log(LogLevel.Error, $"User with id '{userEditDto.Id}' not found");
            return IdentityResult.Failed();
        }

        identityUser.UserName = userEditDto.UserName;
        identityUser.Email = userEditDto.Email;
        identityUser.PhoneNumber = userEditDto.PhoneNumber;
        if (userEditDto.Password != null)
        {
            identityUser.PasswordHash = HashPassword(identityUser, userEditDto.Password);
        }

        var result = await _userManager.UpdateAsync(identityUser);
        if (result.Succeeded)
        {
            _logger.Log(LogLevel.Information, $"User '{identityUser.UserName}' updated");
            return result;
        }

        return result;
    }

    public async Task<IdentityUser> GetCurrentUser()
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            throw new UserException(ErrorCodes.NotAuthenticated, "No signed in user");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new UserException(ErrorCodes.NotFound, $"Current user with id '{userId}' not found");
        }

        return user;
    }

    private string HashPassword(IdentityUser user, string password)
    {
        return _userManager.PasswordHasher.HashPassword(user, password);
    }
}