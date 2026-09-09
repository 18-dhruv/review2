namespace Review5;

public interface IRepository<TKey,TEntity> where TEntity:IEntity<TKey>
{
    TEntity GetById(TKey key);
    void Add(TEntity entity);
    void RemoveById(TKey key);
    List<TEntity> GetAll();
}