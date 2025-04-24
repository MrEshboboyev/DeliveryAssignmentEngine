using DeliveryAssignmentEngine.Domain.Entities;
using DeliveryAssignmentEngine.Domain.ValueObjects;

namespace DeliveryAssignmentEngine.Domain.Repositories;

public interface IDeliveryRepository
{
    Task<Delivery> GetByIdAsync(DeliveryId id);
    Task AddAsync(Delivery delivery);
    Task UpdateAsync(Delivery delivery);
    Task<IEnumerable<Delivery>> GetPendingDeliveriesAsync();
}
