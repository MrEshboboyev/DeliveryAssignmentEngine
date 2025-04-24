using DeliveryAssignmentEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DeliveryAssignmentEngine.Infrastructure.Data;

public class DeliveryDbContext(DbContextOptions<DeliveryDbContext> options) : DbContext(options)
{
    public DbSet<Delivery> Deliveries { get; set; }
    public DbSet<DeliveryAgent> DeliveryAgents { get; set; }
}