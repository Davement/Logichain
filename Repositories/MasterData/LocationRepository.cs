using Microsoft.EntityFrameworkCore;
using Models;
using DbContext = Migrations.DbContext;

namespace Repositories.MasterData;

public interface ILocationRepository
{
    Task<List<T>> GetList<T>() where T : BaseEntity;
    Task<T> GetById<T>(Guid id) where T : BaseEntity;
    Task<T> Add<T>(T entity, bool detachRelations = true) where T : BaseEntity;
    Task<T> Update<T>(T entity, bool detachRelations = true) where T : BaseEntity;
    Task<int> Delete<T>(T entity) where T : BaseEntity;
    Task<bool> CheckIfItemExists<T>(Guid id) where T : BaseEntity;
}

public class LocationRepository : BaseRepository, ILocationRepository
{
    public LocationRepository(DbContext dbContext) : base(dbContext)
    {
    }
}