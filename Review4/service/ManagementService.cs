using Review4.models;

namespace Review4.service;

public class ManagementService
{
  public readonly HashMapRepo<int, Book> Catalog = new HashMapRepo<int, Book>();
  public readonly HashMapRepo<int, Member> Members = new HashMapRepo<int, Member>();
}
