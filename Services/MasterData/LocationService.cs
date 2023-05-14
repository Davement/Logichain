using Dtos;
using Mapster;
using Models;
using Repositories.MasterData;

namespace Services.MasterData;

public interface ILocationService
{
    LocationInfoDto CreateLocation(LocationEditDto locationEditDto);
    IList<LocationInfoDto> GetLocations();
    LocationInfoDto GetLocationById(Guid id);
    LocationInfoDto UpdateLocation(LocationEditDto locationEditDto);
    bool DeleteLocation(Guid id);
}

public class LocationService : ILocationService
{
    private readonly ILocationRepository _locationRepository;

    public LocationService(ILocationRepository locationRepository)
    {
        _locationRepository = locationRepository;
    }

    public IList<LocationInfoDto> GetLocations()
    {
        var locations = _locationRepository.GetList<Location>();
        return locations.Adapt<IList<LocationInfoDto>>();
    }

    public LocationInfoDto GetLocationById(Guid id)
    {
        var location = _locationRepository.GetById<Location>(id);
        return location.Adapt<LocationInfoDto>();
    }

    public LocationInfoDto CreateLocation(LocationEditDto locationEditDto)
    {
        var location = locationEditDto.Adapt<Location>();
        var result = _locationRepository.Add(location);
        return result.Adapt<LocationInfoDto>();
    }

    public LocationInfoDto UpdateLocation(LocationEditDto locationEditDto)
    {
        var location = locationEditDto.Adapt<Location>();
        var result = _locationRepository.Update(location);
        return result.Adapt<LocationInfoDto>();
    }

    public bool DeleteLocation(Guid id)
    {
        _locationRepository.Delete(new Location { Id = id });
        return true;
    }
}