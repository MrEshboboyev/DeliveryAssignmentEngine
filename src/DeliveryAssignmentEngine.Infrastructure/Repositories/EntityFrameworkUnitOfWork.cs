using DeliveryAssignmentEngine.Domain.Repositories;
using DeliveryAssignmentEngine.Infrastructure.Data;

namespace DeliveryAssignmentEngine.Infrastructure.Repositories;

public class EntityFrameworkUnitOfWork(DeliveryDbContext dbContext) : IUnitOfWork
{
    public async Task SaveChangesAsync()
    {
        await dbContext.SaveChangesAsync();
    }
}
