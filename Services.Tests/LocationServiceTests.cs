using Dtos;
using FluentAssertions;
using Mapster;
using Microsoft.Extensions.Logging;
using Models;
using Moq;
using Repositories.MasterData;
using Services.MasterData;

namespace Services.Tests;

[TestFixture]
public class LocationServiceTests
{
    private Mock<ILogger<LocationService>> _loggerMock;
    private Mock<ILocationRepository> _locationRepositoryMock;
    private ILocationService _locationService;

    [SetUp]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<LocationService>>();
        _locationRepositoryMock = new Mock<ILocationRepository>();

        _locationService = new LocationService(
            _loggerMock.Object,
            _locationRepositoryMock.Object
        );
    }

    [Test]
    public async Task WhenGetLocation_ThenShouldReturnLocations()
    {
        // Arrange
        var locations = new List<Location>
        {
            CreateLocationModel(1, "Location 1"),
            CreateLocationModel(2, "Location 2")
        };
        _locationRepositoryMock.Setup(r => r.GetList<Location>()).ReturnsAsync(locations);

        // Act
        var result = await _locationService.GetLocations();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(locations.Count);
        result.Should().ContainItemsAssignableTo<LocationInfoDto>();
    }

    [Test]
    public async Task WhenGetLocationById_ThenShouldReturnLocation()
    {
        // Arrange
        var location = CreateLocationModel(1, "Location 1");
        _locationRepositoryMock.Setup(r => r.GetById<Location>(location.Id)).ReturnsAsync(location);

        // Act
        var result = await _locationService.GetLocationById(location.Id);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<LocationInfoDto>();
    }

    [Test]
    public async Task WhenCreateLocationWithValidData_ThenShouldReturnNewLocationId()
    {
        // Arrange
        var locationEditDto = CreateLocationEditDto(1, "Location 1");
        var location = locationEditDto.Adapt<Location>();
        var createdLocation = CreateLocationModel(1, "Location 1");
        location.Parent = new Location { Id = locationEditDto.ParentId };

        _locationRepositoryMock.Setup(r => r.CheckIfItemExists<Location>(locationEditDto.ParentId)).ReturnsAsync(true);
        _locationRepositoryMock.Setup(r => r.Add(It.IsAny<Location>(), true)).ReturnsAsync(createdLocation);

        // Act
        var result = await _locationService.CreateLocation(locationEditDto);

        // Assert
        result.Should().Be(createdLocation.Id);
        _locationRepositoryMock.Verify(r => r.CheckIfItemExists<Location>(locationEditDto.ParentId), Times.Once);
        _locationRepositoryMock.Verify(r => r.Add(It.IsAny<Location>(), true), Times.Once);
    }

    [Test]
    public async Task WhenCreateLocationWithEmptyGuidAsParent_ThenAccepted()
    {
        // Arrange
        var locationEditDto = CreateLocationEditDto(1, "Location 1");
        locationEditDto.ParentId = Guid.Empty;

        // Act
        var act = async () => await _locationService.CreateLocation(locationEditDto);

        // Assert
        await act.Should().NotThrowAsync<UserException>();
        _locationRepositoryMock.Verify(r => r.CheckIfItemExists<Location>(locationEditDto.ParentId), Times.Never);
        _locationRepositoryMock.Verify(r => r.Add(It.IsAny<Location>(), true), Times.Once);
    }

    [Test]
    public async Task WhenCreateLocationWithNonExistentParent_ThenThrowUserException()
    {
        // Arrange
        var locationEditDto = CreateLocationEditDto(1, "Location 1");
        locationEditDto.ParentId = Guid.NewGuid();
        _locationRepositoryMock.Setup(r => r.CheckIfItemExists<Location>(locationEditDto.ParentId)).ReturnsAsync(false);

        // Act
        var act = async () => await _locationService.CreateLocation(locationEditDto);

        // Assert
        await act.Should().ThrowAsync<UserException>()
            .WithMessage($"Parent location not found with id '{locationEditDto.ParentId}'");
        _locationRepositoryMock.Verify(r => r.CheckIfItemExists<Location>(locationEditDto.ParentId), Times.Once);
        _locationRepositoryMock.Verify(r => r.Add(It.IsAny<Location>(), true), Times.Never);
    }

    [Test]
    public async Task WhenUpdateLocationWithValidData_ThenShouldReturnUpdatedLocationId()
    {
        // Arrange
        var locationEditDto = CreateLocationEditDto(1, "Location 1");
        var location = locationEditDto.Adapt<Location>();
        var updatedLocation = CreateLocationModel(1, "Location 1");

        _locationRepositoryMock.Setup(r => r.CheckIfItemExists<Location>(locationEditDto.ParentId)).ReturnsAsync(true);
        _locationRepositoryMock.Setup(r => r.Update(It.IsAny<Location>(), true)).ReturnsAsync(updatedLocation);

        // Act
        var result = await _locationService.UpdateLocation(locationEditDto);

        // Assert
        result.Should().Be(updatedLocation.Id);
        _locationRepositoryMock.Verify(r => r.CheckIfItemExists<Location>(locationEditDto.ParentId), Times.Once);
        _locationRepositoryMock.Verify(r => r.Update(It.IsAny<Location>(), true), Times.Once);
    }

    [Test]
    public async Task WhenUpdateLocationWithEmptyGuidAsParent_ThenAccepted()
    {
        // Arrange
        var locationEditDto = CreateLocationEditDto(1, "Location 1");
        locationEditDto.ParentId = Guid.Empty;

        // Act
        var act = async () => await _locationService.UpdateLocation(locationEditDto);

        // Assert
        await act.Should().NotThrowAsync<UserException>();
        _locationRepositoryMock.Verify(r => r.CheckIfItemExists<Location>(locationEditDto.ParentId), Times.Never);
        _locationRepositoryMock.Verify(r => r.Update(It.IsAny<Location>(), true), Times.Once);
    }

    [Test]
    public async Task WhenUpdateLocationWithNonExistentParent_ThenThrowUserException()
    {
        // Arrange
        var locationEditDto = CreateLocationEditDto(1, "Location 1");
        locationEditDto.ParentId = Guid.NewGuid();
        _locationRepositoryMock.Setup(r => r.CheckIfItemExists<Location>(locationEditDto.ParentId)).ReturnsAsync(false);

        // Act
        var act = async () => await _locationService.UpdateLocation(locationEditDto);

        // Assert
        await act.Should().ThrowAsync<UserException>()
            .WithMessage($"Parent location not found with id '{locationEditDto.ParentId}'");
        _locationRepositoryMock.Verify(r => r.CheckIfItemExists<Location>(locationEditDto.ParentId), Times.Once);
        _locationRepositoryMock.Verify(r => r.Update(It.IsAny<Location>(), true), Times.Never);
    }

    [Test]
    public async Task WhenDeleteLocation_ThenAccepted()
    {
        // Arrange
        var locationId = Guid.NewGuid();
        _locationRepositoryMock.Setup(r => r.Delete<Location>(new Location { Id = locationId }));

        // Act
        var result = await _locationService.DeleteLocation(locationId);

        // Assert
        result.Should().Be(true);
    }

    private static LocationEditDto CreateLocationEditDto(int number, string name)
    {
        return new LocationEditDto
        {
            Id = Guid.NewGuid(),
            Number = number,
            Name = name,
            LocationType = LocationTypes.Establishment,
            ParentId = Guid.NewGuid(),
            Active = true,
        };
    }

    private static Location CreateLocationModel(int number, string name)
    {
        return new Location
        {
            Id = Guid.NewGuid(),
            Number = number,
            Name = name,
            LocationType = LocationTypes.Establishment,
            Active = true,
            Created = DateTime.Now,
            Updated = DateTime.Now,
        };
    }
}