using System.ComponentModel.DataAnnotations;
using Models;
#pragma warning disable CS8618

namespace Dtos;

public class LocationInfoDto
{
    [Required] public Guid Id { get; set; }
    [Required] public int Number { get; set; }
    [Required] public string Name { get; set; }
    [Required] public LocationTypes LocationType { get; set; }
    [Required] public LocationInfoDto Parent { get; set; }
    [Required] public bool Active { get; set; }

    [Required] public string DisplayString => Parent?.Name != null ? $"{Name} - {Parent.Name}" : Name;
}