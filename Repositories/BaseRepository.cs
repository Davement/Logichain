using Common.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Models;
using DbContext = Migrations.DbContext;

namespace Repositories;

public abstract class BaseRepository
{
    private readonly DbContext _dbContext;

    protected BaseRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public virtual async Task<List<T>> GetList<T>() where T : BaseEntity
    {
        return await _dbContext.Set<T>().ToListAsync();
    }

    public virtual async Task<T> GetById<T>(Guid id) where T : BaseEntity
    {
        var result = await _dbContext.Set<T>().FindAsync(id);
        if (result == null)
        {
            throw new UserException(ErrorCodes.NotFound, $"{typeof(T).Name} with id '{id}' not found");
        }

        return result;
    }

    public virtual async Task<T> Add<T>(T entity, bool detachRelations = true) where T : BaseEntity
    {
        _dbContext.Set<T>().Add(entity);

        if (detachRelations)
        {
            _dbContext.DetachRelations(entity);
        }

        await _dbContext.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<T> Update<T>(T entity, bool detachRelations = true) where T : BaseEntity
    {
        _dbContext.Set<T>().Update(entity);

        if (detachRelations)
        {
            _dbContext.DetachRelations(entity);
        }

        UpdateBaseEntity(entity);

        await _dbContext.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<int> Delete<T>(T entity) where T : BaseEntity
    {
        _dbContext.Set<T>().Remove(entity);
        var result = await _dbContext.SaveChangesAsync();
        if (result < 1)
        {
            throw new UserException(ErrorCodes.NotFound, $"{typeof(T).Name} with id '{entity.Id}' not found");
        }

        return result;
    }

    public virtual async Task<bool> CheckIfItemExists<T>(Guid id) where T : BaseEntity
    {
        return await _dbContext.Set<T>().AnyAsync(x => x.Id == id);
    }

    private void UpdateBaseEntity<T>(T entity) where T : BaseEntity
    {
        entity.IsUpdated();
        _dbContext.Entry(entity).Property(x => entity.Created).IsModified = false;
    }
}