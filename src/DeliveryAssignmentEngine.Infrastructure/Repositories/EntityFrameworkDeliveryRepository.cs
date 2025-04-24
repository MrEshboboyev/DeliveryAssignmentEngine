using DeliveryAssignmentEngine.Domain.Entities;
using DeliveryAssignmentEngine.Domain.Enums;
using DeliveryAssignmentEngine.Domain.Repositories;
using DeliveryAssignmentEngine.Domain.ValueObjects;
using DeliveryAssignmentEngine.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DeliveryAssignmentEngine.Infrastructure.Repositories;

public class EntityFrameworkDeliveryRepository(DeliveryDbContext dbContext) : IDeliveryRepository
{
    public async Task<Delivery> GetByIdAsync(DeliveryId id)
    {
        return await dbContext.Deliveries
            .FirstOrDefaultAsync(d => d.Id.Value == id.Value);
    }

    public async Task AddAsync(Delivery delivery)
    {
        await dbContext.Deliveries.AddAsync(delivery);
    }

    public Task UpdateAsync(Delivery delivery)
    {
        dbContext.Entry(delivery).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<Delivery>> GetPendingDeliveriesAsync()
    {
        return await dbContext.Deliveries
            .Where(d => d.Status == DeliveryStatus.Created || d.Status == DeliveryStatus.PendingAssignment)
            .ToListAsync();
    }
}
