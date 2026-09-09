namespace Review5;

public interface IEntity<TKey>
{
    TKey Id { get; set; }
}