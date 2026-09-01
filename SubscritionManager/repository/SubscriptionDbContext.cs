using Microsoft.EntityFrameworkCore;
using SubscritionManager;

namespace SubscritionManager.repository;

public class SubscriptionDbContext:DbContext
{
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<Plan> Plans => Set<Plan>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
       optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=SubscriptionDb;Username=postgres;Password=password");
    }
}

// *[master][~/Desktop/snakeAndLadder/SubscritionManager/SubscritionManager]$ docker run --name SubscriptionDb \
// -e POSTGRES_USER=postgres \
// -e POSTGRES_PASSWORD=password \
// -e POSTGRES_DB=SubscriptionDb \
// -p 5432:5432 \
// -d postgres:16
