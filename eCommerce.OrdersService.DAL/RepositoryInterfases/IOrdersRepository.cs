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
    /// <param name="filter"></param>
    /// <returns>list of orders that matched the filter</returns>
    Task<IEnumerable<Order>> GetOrdersByConditionAsync(FilterDefinition<Order> filter);
    
    /// <summary>
    /// Get an order by filter
    /// </summary>
    /// <param name="filter"></param>
    /// <returns>an object that matched the filter</returns>
    Task<Order?> GetOrderByConditionAsync(FilterDefinition<Order>  filter);
    
    /// <summary>
    /// Get all orders with pagination
    /// </summary>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <returns>Get all orders in range</returns>
    Task<IEnumerable<Order>> GetAllAsync(int pageNumber, int pageSize);
    
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