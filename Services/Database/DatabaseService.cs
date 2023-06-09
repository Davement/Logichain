using Dtos;
using Dtos.MasterData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;
using Services.MasterData;
using Services.User;
using DbContext = Migrations.DbContext;

namespace Services.Database;

public class DatabaseService
{
    private readonly ILogger<DatabaseService> _logger;
    private readonly DbContext _dbContext;
    private readonly IUserService _userService;
    private readonly ILocationService _locationService;

    private bool IsDatabaseNew { get; set; }

    public DatabaseService(ILogger<DatabaseService> logger, DbContext dbContext, IUserService userService, ILocationService locationService)
    {
        _logger = logger;
        _dbContext = dbContext;
        _userService = userService;
        _locationService = locationService;
        CheckDatabaseConnection();
    }

    private async void CheckDatabaseConnection()
    {
        _logger.Log(LogLevel.Information, "Connecting database...");
        if (await _dbContext.Database.CanConnectAsync())
        {
            _logger.Log(LogLevel.Information, "Successfully connected to database");
            await CheckIsDatabaseNew();
            await IsDatabaseSchemaValid();
            if (IsDatabaseNew)
            {
                await SeedDatabase();
            }
        }
        else
        {
            throw new UserException(ErrorCodes.DatabaseConnectionError, "Cannot connect to database");
        }
    }

    private async Task IsDatabaseSchemaValid()
    {
        var pendingMigrations = await _dbContext.Database.GetPendingMigrationsAsync();
        if (!pendingMigrations.Any()) return;
        _logger.Log(LogLevel.Information, "Database schema updating...");
        await _dbContext.Database.MigrateAsync();
        _logger.Log(LogLevel.Information, "Database schema updated");
    }

    private async Task CheckIsDatabaseNew()
    {
        var appliedMigrations = await _dbContext.Database.GetAppliedMigrationsAsync();
        IsDatabaseNew = !appliedMigrations?.Any() ?? false;
    }

    private async Task SeedDatabase()
    {
        _logger.Log(LogLevel.Information, "Seeding database...");
        await CreateAdminUser();
        CreateCompanyLocation();
        _logger.Log(LogLevel.Information, "Seeding database successfully");
    }

    private async Task CreateAdminUser()
    {
        await _userService.CreateUser(new UserEditDto
        {
            UserName = "admin",
            Password = "123",
        });
    }

    private void CreateCompanyLocation()
    {
        _locationService.CreateLocation(new LocationEditDto
        {
            Number = 1,
            Name = "Company",
            LocationType = LocationTypes.Company,
            ParentId = Guid.Empty,
            Active = true
        });
    }
}