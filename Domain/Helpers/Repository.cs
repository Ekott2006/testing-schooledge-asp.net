using System.Reflection;
using Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace Domain.Helpers;

public class Repository<T>(DataContext context)
    where T : class
{
    private readonly DbSet<T> _dbSet = context.Set<T>();
    
    public async Task<T> UpsertAsync(T entity, Func<T, bool> predicate)
    {
        T? existingEntity = _dbSet.FirstOrDefault(predicate);

        if (existingEntity == null)
        {
            await _dbSet.AddAsync(entity); // Insert new record
        }
        else
        {
            context.Entry(existingEntity).CurrentValues.SetValues(entity); // Update record
        }

        await context.SaveChangesAsync();
        return entity;
    }


    // Create (Add)
    public async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await context.SaveChangesAsync();
        return entity;
    }

    // Read (Get by ID)
    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    // Read (Get all)
    public async Task<List<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    // Update (Full Update)
    public async Task<bool> UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        return await context.SaveChangesAsync() > 0;
    }

    // Patch (Partial Update)
    public async Task<bool> PatchAsync(int id, Dictionary<string, object> updates)
    {
        T? entity = await _dbSet.FindAsync(id);
        if (entity == null) return false;

        Type entityType = typeof(T);

        foreach (KeyValuePair<string, object> update in updates)
        {
            PropertyInfo? property = entityType.GetProperty(update.Key);
            if (property != null && property.CanWrite)
            {
                property.SetValue(entity, Convert.ChangeType(update.Value, property.PropertyType));
            }
        }

        return await context.SaveChangesAsync() > 0;
    }

    // Delete
    public async Task<bool> DeleteAsync(int id)
    {
        T? entity = await _dbSet.FindAsync(id);
        if (entity == null) return false;

        _dbSet.Remove(entity);
        return await context.SaveChangesAsync() > 0;
    }
}