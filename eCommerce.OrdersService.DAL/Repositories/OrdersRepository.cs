using eCommerce.OrdersService.DAL.Context;
using eCommerce.OrdersService.DAL.Entities;
using eCommerce.OrdersService.DAL.RepositoryInterfases;

namespace eCommerce.OrdersService.DAL.Repositories;

public class OrdersRepository(ApplicationDbContext context) : IOrdersRepository
{
    private readonly ApplicationDbContext  _context = context;
    
    public Task<Order?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<OrderItem>> GetAllAsync(int pageNumber, int pageSize)
    {
        throw new NotImplementedException();
    }

    public Task<Order?> CreateAsync(Order order)
    {
        throw new NotImplementedException();
    }

    public Task<Order?> UpdateAsync(Order order)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}