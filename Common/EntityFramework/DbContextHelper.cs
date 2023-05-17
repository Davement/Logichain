using System.Collections;
using Microsoft.EntityFrameworkCore;

namespace Common.EntityFramework;

public static class DbContextHelper
{
    public static void DetachRelations<T>(this DbContext dbContext, T entity) where T : class
    {
        var entityEntry = dbContext.Entry(entity);
        foreach (var navigation in entityEntry.Navigations)
        {
            if (navigation.CurrentValue == null)
            {
                continue;
            }

            if (navigation.Metadata.IsCollection)
            {
                foreach (var relatedEntity in (IEnumerable)navigation.CurrentValue)
                {
                    dbContext.Entry(relatedEntity).State = EntityState.Unchanged;
                }
            }
            else
            {
                dbContext.Entry(navigation.CurrentValue).State = EntityState.Unchanged;
            }
        }
    }
}