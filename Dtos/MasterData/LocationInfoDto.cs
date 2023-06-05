using System.ComponentModel.DataAnnotations;

#pragma warning disable CS8618

namespace Dtos.MasterData;

public class LocationInfoDto : LocationDto
{
    [Required] public Guid Id { get; set; }
    [Required] public LocationInfoDto Parent { get; set; }

    [Required] public string DisplayString => Parent?.Name != null ? $"{Name} - {Parent.Name}" : Name;
}