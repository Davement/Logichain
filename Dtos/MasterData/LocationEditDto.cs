namespace Dtos.MasterData;

public class LocationEditDto : LocationDto
{
    public Guid Id { get; set; }
    public Guid ParentId { get; set; }
}