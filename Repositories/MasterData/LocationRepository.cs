using Models;
using DbContext = Migrations.DbContext;

namespace Repositories.MasterData;

public interface ILocationRepository
{
    Task<List<Location>> GetList();
    Task<Location> GetById(Guid id);
    Task<Location> Add(Location entity, bool detachRelations = true);
    Task<Location> Update(Location entity, bool detachRelations = true);
    Task<int> Delete(Location entity);
    Task<bool> CheckIfItemExists(Guid id);
}

public class LocationRepository : BaseRepository<Location>, ILocationRepository
{
    public LocationRepository(DbContext dbContext) : base(dbContext)
    {
    }
}