using Microsoft.EntityFrameworkCore;
using SubscritionManager.repository;

namespace SubscritionManager;

public class Repository<TKey,TEntity>:IRepository<TKey,TEntity> where TEntity:class, IEntity<TKey>
{
    private readonly SubscriptionDbContext _dbContext;
    private readonly DbSet<TEntity> DbSet;
    public Repository(SubscriptionDbContext dbContext)
    {
        this._dbContext = dbContext;
        this.DbSet = dbContext.Set<TEntity>();
    }
    public void Add(TEntity entity)
    {
        DbSet.Add(entity);
        _dbContext.SaveChanges();
    }

    public void RemoveById(TKey key)
    {
        var entity = GetById(key);
        DbSet.Remove(entity);
        _dbContext.SaveChanges();
    }

    public TEntity GetById(TKey key)
    {
        var entity = DbSet.Find(key);
        if(entity==null) throw new KeyNotFoundException($"{typeof(TEntity).Name} with key '{key}' was not found.");
        return entity;
    }

    public List<KeyValuePair<TKey, TEntity>> GetAll()
    {
        return DbSet.Select(x => new KeyValuePair<TKey, TEntity>(x.Id, x)).ToList();
    }
}
