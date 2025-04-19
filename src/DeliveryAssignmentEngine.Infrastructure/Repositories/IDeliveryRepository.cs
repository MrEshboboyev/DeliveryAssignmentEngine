namespace DeliveryAssignmentEngine.Infrastructure.Repositories;

public interface IDeliveryRepository
{
    Task<Delivery> GetByIdAsync(DeliveryId id);
    Task AddAsync(Delivery delivery);
    Task UpdateAsync(Delivery delivery);
    Task<IEnumerable<Delivery>> GetPendingDeliveriesAsync();
}

public interface IDeliveryAgentRepository
{
    Task<DeliveryAgent> GetByIdAsync(DeliveryAgentId id);
    Task AddAsync(DeliveryAgent agent);
    Task UpdateAsync(DeliveryAgent agent);
    Task<IEnumerable<DeliveryAgent>> GetAvailableAgentsAsync();
    Task<IEnumerable<DeliveryAgent>> GetAgentsInAreaAsync(ServiceArea area);
}

public interface IUnitOfWork
{
    Task SaveChangesAsync();
}

public class EntityFrameworkDeliveryRepository : IDeliveryRepository
{
    private readonly DeliveryDbContext _dbContext;

    public EntityFrameworkDeliveryRepository(DeliveryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Delivery> GetByIdAsync(DeliveryId id)
    {
        return await _dbContext.Deliveries
            .FirstOrDefaultAsync(d => d.Id.Value == id.Value);
    }

    public async Task AddAsync(Delivery delivery)
    {
        await _dbContext.Deliveries.AddAsync(delivery);
    }

    public Task UpdateAsync(Delivery delivery)
    {
        _dbContext.Entry(delivery).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<Delivery>> GetPendingDeliveriesAsync()
    {
        return await _dbContext.Deliveries
            .Where(d => d.Status == DeliveryStatus.Created || d.Status == DeliveryStatus.PendingAssignment)
            .ToListAsync();
    }
}

public class EntityFrameworkDeliveryAgentRepository : IDeliveryAgentRepository
{
    private readonly DeliveryDbContext _dbContext;

    public EntityFrameworkDeliveryAgentRepository(DeliveryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DeliveryAgent> GetByIdAsync(DeliveryAgentId id)
    {
        return await _dbContext.DeliveryAgents
            .FirstOrDefaultAsync(a => a.Id.Value == id.Value);
    }

    public async Task AddAsync(DeliveryAgent agent)
    {
        await _dbContext.DeliveryAgents.AddAsync(agent);
    }

    public Task UpdateAsync(DeliveryAgent agent)
    {
        _dbContext.Entry(agent).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<DeliveryAgent>> GetAvailableAgentsAsync()
    {
        return await _dbContext.DeliveryAgents
            .Where(a => a.Status == DeliveryAgentStatus.Available)
            .ToListAsync();
    }

    public async Task<IEnumerable<DeliveryAgent>> GetAgentsInAreaAsync(ServiceArea area)
    {
        // This would use spatial queries in a real implementation
        // Simplified for demonstration
        return await _dbContext.DeliveryAgents
            .Where(a => a.Status == DeliveryAgentStatus.Available)
            .ToListAsync();
    }
}

public class EntityFrameworkUnitOfWork : IUnitOfWork
{
    private readonly DeliveryDbContext _dbContext;

    public EntityFrameworkUnitOfWork(DeliveryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}