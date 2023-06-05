namespace Models;

public class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }

    public void UpdateTimestamp()
    {
        Updated = DateTime.UtcNow;
    }
}