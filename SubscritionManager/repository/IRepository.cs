namespace SubscritionManager;

public interface IRepository<TKey,TEntity> where TEntity:class,IEntity<TKey>
{
     void Add(TEntity entity);
     void RemoveById(TKey key);
     TEntity GetById(TKey key);
     List<KeyValuePair<TKey, TEntity>> GetAll();
}