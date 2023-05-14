using Migrations;

namespace Repositories;

public abstract class BaseRepository
{
    private readonly DbContext _dbContext;

    protected BaseRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IList<T> GetList<T>() where T : class
    {
        return _dbContext.Set<T>().AsQueryable().ToList();
    }

    public T GetById<T>(Guid id) where T : class
    {
        var result = _dbContext.Set<T>().Find(id);
        if (result == null)
        {
            throw new ArgumentException($"{nameof(T)} not found");
        }

        return result;
    }

    public T Add<T>(T entity) where T : class
    {
        _dbContext.Set<T>().Add(entity);
        _dbContext.SaveChanges();
        return entity;
    }

    public T Update<T>(T entity) where T : class
    {
        _dbContext.Set<T>().Update(entity);
        _dbContext.SaveChanges();
        return entity;
    }

    public T Delete<T>(T entity) where T : class
    {
        _dbContext.Set<T>().Remove(entity);
        _dbContext.SaveChanges();
        return entity;
    }
}