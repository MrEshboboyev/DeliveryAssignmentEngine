using DeliveryAssignmentEngine.Domain.Repositories;

namespace DeliveryAssignmentEngine.Infrastructure.Repositories;

public class EntityFrameworkUnitOfWork(DeliveryDbContext dbContext) : IUnitOfWork
{
    public async Task SaveChangesAsync()
    {
        await dbContext.SaveChangesAsync();
    }
}
