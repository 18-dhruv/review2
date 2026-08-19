namespace SubdomainAnalyser;

public interface IRepository<TKey,TEntity>where TEntity : class
{
    void Add(TKey key ,TEntity entity){}
}