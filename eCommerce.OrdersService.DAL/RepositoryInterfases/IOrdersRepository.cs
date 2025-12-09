using System.Linq.Expressions;
using eCommerce.OrdersService.DAL.Entities;
using MongoDB.Driver;

namespace eCommerce.OrdersService.DAL.RepositoryInterfases;

public interface IOrdersRepository
{
    /// <summary>
    /// Get Order by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>A order or null</returns>
    Task<Order?> GetByIdAsync(Guid id);

    /// <summary>
    /// Get list of orders by filter
    /// </summary>
    /// <param name="filterDto"></param>
    /// <returns>list of orders that matched the filter</returns>
    Task<IEnumerable<Order>> GetOrdersByConditionAsync(OrderFilter filterDto);

    /// <summary>
    /// Create new order
    /// </summary>
    /// <param name="order"></param>
    /// <returns>new order or null</returns>
    Task<Order?> CreateAsync(Order order);

    /// <summary>
    /// Update order
    /// </summary>
    /// <param name="order"></param>
    /// <returns>Updated order</returns>
    Task<Order?> UpdateAsync(Order order);

    /// <summary>
    /// Delete order by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>true if order was successfully deleted</returns>
    Task<bool> DeleteAsync(Guid id);
}