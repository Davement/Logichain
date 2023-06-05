using System.ComponentModel.DataAnnotations;
using Models;

#pragma warning disable CS8618

namespace Dtos.MasterData;

public class LocationDto
{
    [Required] public int Number { get; set; }
    [Required] public string Name { get; set; }
    [Required] public LocationTypes LocationType { get; set; }
    [Required] public bool Active { get; set; }
}