namespace SubdomainAnalyser;

public class RepositoryDic<TKey,TEntity>:IRepository<TKey,TEntity> where TEntity : class
{

    public Dictionary<TKey, TEntity> map = new Dictionary<TKey, TEntity>();
    
    public void Add(TKey key,TEntity entity)
    {
      if 
    }
}