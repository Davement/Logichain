using Common.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Models;
using DbContext = Migrations.DbContext;

namespace Repositories;

public abstract class BaseRepository<T> where T : BaseEntity
{
    private readonly DbContext _dbContext;

    protected BaseRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public virtual async Task<List<T>> GetList()
    {
        return await _dbContext.Set<T>().ToListAsync();
    }

    public virtual async Task<T> GetById(Guid id)
    {
        var result = await _dbContext.Set<T>().FindAsync(id);
        if (result == null)
        {
            throw new UserException(ErrorCodes.NotFound, $"{typeof(T).Name} with id '{id}' not found");
        }

        return result;
    }

    public virtual async Task<T> Add(T entity, bool detachRelations = true)
    {
        _dbContext.Set<T>().Add(entity);

        if (detachRelations)
        {
            _dbContext.DetachRelations(entity);
        }

        await _dbContext.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<T> Update(T entity, bool detachRelations = true)
    {
        _dbContext.Set<T>().Update(entity);

        if (detachRelations)
        {
            _dbContext.DetachRelations(entity);
        }
        else
        {
            UpdateRelationalBaseEntities(entity);
        }

        UpdateBaseEntity(entity);

        await _dbContext.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<int> Delete(T entity)
    {
        _dbContext.Set<T>().Remove(entity);
        var result = await _dbContext.SaveChangesAsync();
        if (result < 1)
        {
            throw new UserException(ErrorCodes.NotFound, $"{typeof(T).Name} with id '{entity.Id}' not found");
        }

        return result;
    }

    public virtual async Task<bool> CheckIfItemExists(Guid id)
    {
        return await _dbContext.Set<T>().AnyAsync(x => x.Id == id);
    }

    private void UpdateBaseEntity(BaseEntity entity)
    {
        entity.IsUpdated();
        _dbContext.Entry(entity).Property(x => x.Created).IsModified = false;
    }

    private void UpdateRelationalBaseEntities(T entity)
    {
        var properties = typeof(T).GetProperties();

        foreach (var property in properties)
        {
            if (typeof(BaseEntity).IsAssignableFrom(property.PropertyType))
            {
                if (property.GetValue(entity) is BaseEntity childEntity)
                {
                    UpdateBaseEntity(childEntity);
                }
            }
            else if (typeof(IEnumerable<BaseEntity>).IsAssignableFrom(property.PropertyType))
            {
                if (property.GetValue(entity) is not IEnumerable<BaseEntity> childEntities)
                {
                    continue;
                }

                foreach (var childEntity in childEntities)
                {
                    UpdateBaseEntity(childEntity);
                }
            }
        }
    }
}