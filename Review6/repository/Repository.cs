using Microsoft.EntityFrameworkCore;
using Review5.exception;

namespace Review5
{
    public class Repository<TKey, TEntity> :
        IRepository<TKey, TEntity>
        where TEntity : class, IEntity<TKey>
    {
        private readonly MeterDbContext DbContext;
        private readonly DbSet<TEntity> DbSet;

        public Repository(MeterDbContext dbContext)
        {
            DbContext = dbContext;
            DbSet = DbContext.Set<TEntity>();
        }

        public TEntity GetById(TKey key)
        {
            var entity = DbSet.Find(key);

            if (entity == null)
            {
                throw new EntityDontExist(
                    $"Entity associated with key {key} doesn't exist"
                );
            }

            return entity;
        }

        public void Add(TEntity entity)
        {
            try
            {
                DbSet.Add(entity);
                DbContext.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(
                    $"Database error: {ex.InnerException?.Message ?? ex.Message}"
                );

                throw;
            }
        }

        public void RemoveById(TKey key)
        {
            var entity = GetById(key);

            DbSet.Remove(entity);

            DbContext.SaveChanges();
        }

        public List<TEntity> GetAll()
        {
            return DbSet.ToList();
        }
    }
}