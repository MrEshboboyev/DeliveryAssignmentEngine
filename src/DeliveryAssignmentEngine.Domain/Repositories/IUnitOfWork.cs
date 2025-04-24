namespace DeliveryAssignmentEngine.Domain.Repositories;

public interface IUnitOfWork
{
    Task SaveChangesAsync();
}