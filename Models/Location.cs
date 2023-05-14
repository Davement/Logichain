#pragma warning disable CS8618

namespace Models;

public class Location : BaseEntity
{
    public int Number { get; set; }
    public string Name { get; set; }
    public LocationTypes LocationType { get; set; }
    public Location? Parent { get; set; }
    public bool Active { get; set; }
}