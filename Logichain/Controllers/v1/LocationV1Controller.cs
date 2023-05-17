using Dtos;
using Microsoft.AspNetCore.Mvc;
using Services.MasterData;

namespace Logichain.Controllers.v1;

[Route("api/v1/locations")]
[Produces("application/json")]
public class LocationV1Controller : BaseController
{
    private readonly ILocationService _locationService;

    public LocationV1Controller(ILocationService locationService)
    {
        _locationService = locationService;
    }
    
    [HttpGet]
    public async Task<IList<LocationInfoDto>> GetLocations()
    {
        return await _locationService.GetLocations();
    }
    
    [HttpGet("{id:guid}")]
    public async Task<LocationInfoDto> GetLocationById(Guid id)
    {
        return await _locationService.GetLocationById(id);
    }

    [HttpPost]
    public async Task<IActionResult> CreateLocation(LocationEditDto locationEditDto)
    {
        var result = await _locationService.CreateLocation(locationEditDto);
        return Created(result);
    }
    
    [HttpPut]
    public async Task<Guid> UpdateLocation(LocationEditDto locationEditDto)
    {
        return await _locationService.UpdateLocation(locationEditDto);
    }
    
    [HttpDelete("{id:guid}")]
    public async Task DeleteLocation(Guid id)
    {
        await _locationService.DeleteLocation(id);
    }
}