using System.ComponentModel.DataAnnotations;
using Models;
#pragma warning disable CS8618

namespace Dtos;

public class LocationEditDto
{
    public Guid Id { get; set; }
    [Required] public int Number { get; set; }
    [Required] public string Name { get; set; }
    [Required] public LocationTypes LocationType { get; set; }
    public Guid ParentId { get; set; }
    [Required] public bool Active { get; set; }
}