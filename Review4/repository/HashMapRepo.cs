using Review4.Exceptions;

namespace Review4.models;

public class HashMapRepo<TKey,TEntity> where TKey : notnull where TEntity : class
{
   internal Dictionary<TKey, TEntity> HashMap = new Dictionary<TKey, TEntity>();

   public IEnumerable<TEntity>Values => HashMap.Values;

   public void AddToHashMap(TKey key, TEntity entity)
   {
      if ( !HashMap.ContainsKey(key))
      {
        HashMap.Add(key,entity);
      }
         else
         {
             throw new AlreadyExistInHashMap($"{key} already exist");
         }
   }

   public bool check(TKey key)
   {
       return  HashMap.ContainsKey(key);
   }
}
