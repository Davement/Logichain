using System.Reflection;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Models;
using Repositories.MasterData;

namespace Repositories.Test;

[TestFixture]
public class BaseRepositoryTests
{
    private IConfiguration _configuration;
    private DbContextOptions<Migrations.DbContext> _dbContextOptions;
    private Migrations.DbContext _dbContext;
    private LocationRepository _locationRepository;
    private Location _createdLocation;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        var currentPath = Directory.GetCurrentDirectory();
        var currentDirectoryName = Assembly.GetExecutingAssembly().GetName().Name!;
        var appsettingsPath = currentPath.Replace(currentDirectoryName, "Logichain") + "/appsettings.json";

            _configuration = new ConfigurationManager()
            .AddJsonFile(appsettingsPath)
            .Build();
        
        _dbContextOptions = new DbContextOptions<Migrations.DbContext>();
        _dbContext = new Migrations.DbContext(_dbContextOptions, _configuration, "LogichainTestDb");
        _locationRepository = new LocationRepository(_dbContext);

        if (!await _dbContext.Database.CanConnectAsync())
        {
            throw new InconclusiveException("Cannot connect to test database.");
        }

        await _dbContext.Database.MigrateAsync();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _dbContext.Database.ExecuteSqlRaw("DELETE FROM Locations");
    }

    [Test, Order(1)]
    public async Task WhenAddingEntity_ThenShouldBeSavedToDatabase()
    {
        // Arrange
        var location = CreateLocationModel();

        // Act
        var entity = await _locationRepository.Add(location);
        location.Id = entity.Id;
        location.Created = entity.Created;
        location.Updated = entity.Updated;
        _createdLocation = location;

        //Assert
        var savedLocation = await _dbContext.Set<Location>().FindAsync(location.Id);
        savedLocation.Should().NotBeNull();
        savedLocation.Should().BeEquivalentTo(location);
    }

    [Test, Order(2)]
    public async Task WhenGetList_ThenShouldReturnList()
    {
        // Arrange
        var location = CreateLocationModel();
        var entity = await _locationRepository.Add(location);
        
        // Act
        var result = await _locationRepository.GetList();
        
        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(x => x.Id == entity.Id);
        result.Should().Contain(x => x.Id == _createdLocation.Id);
    }

    [Test, Order(3)]
    public async Task WhenGetById_ThenShouldReturnItem()
    {
        // Act
        var result = await _locationRepository.GetById(_createdLocation.Id);
        
        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(_createdLocation);
    }

    [Test, Order(4)]
    public async Task WhenUpdateEntity_ThenShouldBeSavedToDatabase()
    {
        // Arrange
        var originalLocation = new Location
        {
            Id = _createdLocation.Id,
            Number = _createdLocation.Number,
            Name = _createdLocation.Name,
            Parent = _createdLocation.Parent,
            Created = _createdLocation.Created,
            Updated = _createdLocation.Updated
        };
        
        var location = _createdLocation;
        location.Name = "Updated Location";
        
        // Act
        await _locationRepository.Update(location);
        
        // Assert
        var updatedLocation = await _dbContext.Set<Location>().FindAsync(location.Id);
        updatedLocation.Should().NotBeNull();
        updatedLocation!.Id.Should().Be(originalLocation.Id);
        updatedLocation.Number.Should().Be(originalLocation.Number);
        updatedLocation.Name.Should().Be("Updated Location");
        updatedLocation.Parent.Should().BeNull();
        updatedLocation.Created.Should().Be(originalLocation.Created);
        updatedLocation.Updated.Should().NotBe(originalLocation.Updated);
        updatedLocation.Updated.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
    }

    [Test, Order(5)]
    public async Task WhenDeleteEntity_ThenShouldBeDeletedFromDatabase()
    {
        // Arrange
        var locationId = _createdLocation.Id;
        var locationsCount = await _dbContext.Set<Location>().CountAsync();
        
        // Act
        var result = await _locationRepository.Delete(_createdLocation);
        
        // Assert
        result.Should().Be(1);
        
        var location = await _dbContext.Set<Location>().FindAsync(locationId);
        location.Should().BeNull();
        
        var countAfterDelete = await _dbContext.Set<Location>().CountAsync();
        countAfterDelete.Should().Be(locationsCount - 1);
    }

    [Test, Order(6)]
    public async Task WhenAddingEntityWithRelationsAttached_ThenRelationShouldBeSavedToDatabase()
    {
        // Arrange
        var location = CreateLocationModel();
        location.Parent = CreateLocationModel();

        // Act
        var entity = await _locationRepository.Add(location, false);
        location.Id = entity.Id;
        location.Created = entity.Created;
        location.Updated = entity.Updated;
        location.Parent = entity.Parent;
        _createdLocation = location;

        //Assert
        var savedLocation = await _dbContext.Set<Location>().FindAsync(location.Id);
        savedLocation.Should().NotBeNull();
        savedLocation!.Parent.Should().NotBeNull();
        savedLocation.Should().BeEquivalentTo(location);

        var parentLocation = await _dbContext.Set<Location>().FindAsync(location.Parent?.Id);
        parentLocation.Should().NotBeNull();
        parentLocation.Should().BeEquivalentTo(location.Parent);
    }

    [Test, Order(7)]
    public async Task WhenUpdatingRelationEntityWithRelationsAttached_ThenRelationShouldBeSavedToDatabase()
    {
        // Arrange
        var originalLocation = new Location
        {
            Id = _createdLocation.Id,
            Number = _createdLocation.Number,
            Name = _createdLocation.Name,
            Parent = _createdLocation.Parent,
            Created = _createdLocation.Created,
            Updated = _createdLocation.Updated
        };
        
        var originalParentLocation = new Location
        {
            Id = _createdLocation.Parent!.Id,
            Number = _createdLocation.Parent.Number,
            Name = _createdLocation.Parent.Name,
            Parent = _createdLocation.Parent.Parent,
            Created = _createdLocation.Parent.Created,
            Updated = _createdLocation.Parent.Updated
        };
        
        var location = _createdLocation;
        location.Parent.Name = "Updated Parent Location";
        
        // Act
        await _locationRepository.Update(location, false);
        
        // Assert location
        var updatedLocation = await _dbContext.Set<Location>().FindAsync(location.Id);
        updatedLocation.Should().NotBeNull();
        updatedLocation!.Id.Should().Be(originalLocation.Id);
        updatedLocation.Number.Should().Be(originalLocation.Number);
        updatedLocation.Name.Should().Be(originalLocation.Name);
        updatedLocation.Parent.Should().NotBeNull();
        updatedLocation.Created.Should().Be(originalLocation.Created);
        updatedLocation.Updated.Should().NotBe(originalLocation.Updated);
        updatedLocation.Updated.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
        
        // Assert parent location
        var updatedParentLocation = await _dbContext.Set<Location>().FindAsync(location.Parent.Id);
        updatedParentLocation.Should().NotBeNull();
        updatedParentLocation!.Id.Should().Be(originalParentLocation.Id);
        updatedParentLocation.Number.Should().Be(originalParentLocation.Number);
        updatedParentLocation.Name.Should().Be("Updated Parent Location");
        updatedParentLocation.Parent.Should().BeNull();
        updatedParentLocation.Created.Should().Be(originalParentLocation.Created);
        updatedParentLocation.Updated.Should().NotBe(originalParentLocation.Updated);
        updatedParentLocation.Updated.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
    }

    private static Location CreateLocationModel()
    {
        var random = new Random();
        var randomNumber = random.Next(1, 21);
        return new Location
        {
            Number = randomNumber,
            Name = "Location " + randomNumber,
            LocationType = LocationTypes.Establishment,
            Active = true
        };
    }
}