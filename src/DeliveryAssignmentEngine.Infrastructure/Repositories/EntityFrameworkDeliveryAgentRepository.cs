using DeliveryAssignmentEngine.Domain.Entities;
using DeliveryAssignmentEngine.Domain.Enums;
using DeliveryAssignmentEngine.Domain.Repositories;
using DeliveryAssignmentEngine.Domain.ValueObjects;
using DeliveryAssignmentEngine.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DeliveryAssignmentEngine.Infrastructure.Repositories;

public class EntityFrameworkDeliveryAgentRepository(DeliveryDbContext dbContext) : IDeliveryAgentRepository
{
    public async Task<DeliveryAgent> GetByIdAsync(DeliveryAgentId id)
    {
        return await dbContext.DeliveryAgents
            .FirstOrDefaultAsync(a => a.Id.Value == id.Value);
    }

    public async Task AddAsync(DeliveryAgent agent)
    {
        await dbContext.DeliveryAgents.AddAsync(agent);
    }

    public Task UpdateAsync(DeliveryAgent agent)
    {
        dbContext.Entry(agent).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<DeliveryAgent>> GetAvailableAgentsAsync()
    {
        return await dbContext.DeliveryAgents
            .Where(a => a.Status == DeliveryAgentStatus.Available)
            .ToListAsync();
    }

    public async Task<IEnumerable<DeliveryAgent>> GetAgentsInAreaAsync(ServiceArea area)
    {
        // This would use spatial queries in a real implementation
        // Simplified for demonstration
        return await dbContext.DeliveryAgents
            .Where(a => a.Status == DeliveryAgentStatus.Available)
            .ToListAsync();
    }
}
