using Dtos;
using Microsoft.AspNetCore.Mvc;
using Services.MasterData;

namespace Logichain.Controllers.v1;

[Route("api/v1/locations")]
[Produces("application/json")]
public class LocationV1Controller
{
    private readonly ILocationService _locationService;

    public LocationV1Controller(ILocationService locationService)
    {
        _locationService = locationService;
    }
    
    [HttpGet]
    public IList<LocationInfoDto> GetLocations()
    {
        return _locationService.GetLocations();
    }
    
    [HttpGet("{id:guid}")]
    public LocationInfoDto GetLocationById(Guid id)
    {
        return _locationService.GetLocationById(id);
    }

    [HttpPost]
    public void CreateLocation(LocationEditDto locationEditDto)
    {
        _locationService.CreateLocation(locationEditDto);
    }
    
    [HttpPut]
    public void UpdateLocation(LocationEditDto locationEditDto)
    {
        _locationService.UpdateLocation(locationEditDto);
    }
    
    [HttpDelete("{id:guid}")]
    public void DeleteLocation(Guid id)
    {
        _locationService.DeleteLocation(id);
    }
}