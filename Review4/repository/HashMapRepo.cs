namespace Review4.models;

public class HashMapRepo<TKey,TEntity> where TEntity:class
{
   private readonly Dictionary<TKey, TEntity> HashMap = new Dictionary<TKey, TEntity>();

   public void AddToHashMap(TKey key, TEntity entity)
   {
      if(HashMap.ContainsKey(key))throw new 
   }
}