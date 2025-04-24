using DeliveryAssignmentEngine.Domain.Entities;
using DeliveryAssignmentEngine.Domain.ValueObjects;

namespace DeliveryAssignmentEngine.Domain.Repositories;

public interface IDeliveryAgentRepository
{
    Task<DeliveryAgent> GetByIdAsync(DeliveryAgentId id);
    Task AddAsync(DeliveryAgent agent);
    Task UpdateAsync(DeliveryAgent agent);
    Task<IEnumerable<DeliveryAgent>> GetAvailableAgentsAsync();
    Task<IEnumerable<DeliveryAgent>> GetAgentsInAreaAsync(ServiceArea area);
}
