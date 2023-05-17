using Dtos;
using Mapster;
using Microsoft.Extensions.Logging;
using Models;
using Repositories.MasterData;

namespace Services.MasterData;

public interface ILocationService
{
    Task<IList<LocationInfoDto>> GetLocations();
    Task<LocationInfoDto> GetLocationById(Guid id);
    Task<Guid> CreateLocation(LocationEditDto locationEditDto);
    Task<Guid> UpdateLocation(LocationEditDto locationEditDto);
    Task<bool> DeleteLocation(Guid id);
}

public class LocationService : ILocationService
{
    private readonly ILogger<LocationService> _logger;
    private readonly ILocationRepository _locationRepository;

    public LocationService(ILocationRepository locationRepository, ILogger<LocationService> logger)
    {
        _locationRepository = locationRepository;
        _logger = logger;
    }

    public async Task<IList<LocationInfoDto>> GetLocations()
    {
        var locations = await _locationRepository.GetList<Location>();
        return locations.Adapt<IList<LocationInfoDto>>();
    }

    public async Task<LocationInfoDto> GetLocationById(Guid id)
    {
        var location = await _locationRepository.GetById<Location>(id);
        return location.Adapt<LocationInfoDto>();
    }

    public async Task<Guid> CreateLocation(LocationEditDto locationEditDto)
    {
        var location = locationEditDto.Adapt<Location>();
        var parentLocationExists = await _locationRepository.CheckIfItemExists<Location>(locationEditDto.ParentId);
        if (locationEditDto.ParentId != Guid.Empty && parentLocationExists)
        {
            location.Parent = new Location { Id = locationEditDto.ParentId };
        }
        else if (!parentLocationExists)
        {
            throw new UserException(ErrorCodes.ParentLocationNotFound, $"Parent location not found with id '{locationEditDto.ParentId}'");
        }

        var result = await _locationRepository.Add(location);
        _logger.Log(LogLevel.Information, $"Location '{result.Name}' created with id '{result.Id}'");
        return result.Id;
    }

    public async Task<Guid> UpdateLocation(LocationEditDto locationEditDto)
    {
        var location = locationEditDto.Adapt<Location>();
        var parentLocationExists = await _locationRepository.CheckIfItemExists<Location>(locationEditDto.ParentId);
        if (locationEditDto.ParentId != Guid.Empty && parentLocationExists)
        {
            location.Parent = new Location { Id = locationEditDto.ParentId };
        }
        else if (!parentLocationExists)
        {
            throw new UserException(ErrorCodes.ParentLocationNotFound, $"Parent location not found with id '{locationEditDto.ParentId}'");
        }

        var result = await _locationRepository.Update(location);
        _logger.Log(LogLevel.Information, $"Location '{result.Name}' updated with id '{result.Id}'");
        return result.Id;
    }

    public async Task<bool> DeleteLocation(Guid id)
    {
        await _locationRepository.Delete(new Location { Id = id });
        return true;
    }
}