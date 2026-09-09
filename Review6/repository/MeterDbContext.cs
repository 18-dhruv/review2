using Microsoft.EntityFrameworkCore;
using Review5.model;
namespace Review5;

public class MeterDbContext:DbContext
{
    public DbSet<Reading> Readings { get; set; }
    public DbSet<Bill> Bills { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=MeterDb;Username=postgres;Password=password");
    }
}
// *[master][~/Desktop/snakeAndLadder/Review6/Review6]$ docker run --name MeterDb \         
// -e POSTGRES_USERNAME=postgres \
// -e POSTGRES_PASSWORD=password \
// -p 5432:5432 \
// -d postgres:16
// 59f4ac29cd842affac03feb2fd424f4779bf731ede6cd9796ad92d11f8901a86
