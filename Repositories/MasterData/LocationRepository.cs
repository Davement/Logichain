using DbContext = Migrations.DbContext;

namespace Repositories.MasterData;

public interface ILocationRepository
{
    IList<T> GetList<T>() where T : class;
    T GetById<T>(Guid id) where T : class;
    T Add<T>(T entity) where T : class;
    T Update<T>(T entity) where T : class;
    T Delete<T>(T entity) where T : class;
}

public class LocationRepository : BaseRepository, ILocationRepository
{
    public LocationRepository(DbContext dbContext) : base(dbContext)
    {
    }
}