using eCommerce.OrdersService.DAL.Entities;
using eCommerce.OrdersService.DAL.RepositoryInterfases;
using MongoDB.Driver;

namespace eCommerce.OrdersService.DAL.Repositories;

public class OrdersRepository : IOrdersRepository
{
    private readonly IMongoCollection<Order> _orders;
    private readonly string _collectionName = "orders";

    public OrdersRepository(IMongoDatabase db)
    {
        _orders = db.GetCollection<Order>(_collectionName);
    }

    public async Task<Order?> GetByIdAsync(Guid id)
    {
        var filter = Builders<Order>.Filter.Eq(x => x.OrderId, id);
        return await _orders.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Order>> GetOrdersByConditionAsync(FilterDefinition<Order> filter)
    {
        var orders = await _orders.Find(filter).ToListAsync();
        return orders;
    }

    public async Task<Order?> GetOrderByConditionAsync(FilterDefinition<Order> filter)
    {
        var orders = await _orders.Find(filter).FirstOrDefaultAsync();
        return orders;
    }

    public async Task<IEnumerable<Order>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
    {
        if (pageNumber < 1 || pageSize < 1) return new List<Order>();

        var result = await _orders.Find(x => true)
                                            .SortBy(x => x.OrderId)
                                            .Skip((pageNumber - 1) * pageSize)
                                            .Limit(pageSize)
                                            .ToListAsync();

        return result;
    }

    public async Task<Order?> CreateAsync(Order order)
    {
        order.OrderId = Guid.NewGuid();
        await _orders.InsertOneAsync(order);
        return order;
    }

    public async Task<Order?> UpdateAsync(Order order)
    {
        var filter = Builders<Order>.Filter.Eq(x => x.OrderId, order.OrderId);
        
        // One network call. Efficient.
        var result = await _orders.ReplaceOneAsync(filter, order);
    
        // If we found a match, return the NEW object (order), otherwise null.
        return result.MatchedCount > 0 ? order : null;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var filter = Builders<Order>.Filter.Eq(x => x.OrderId, id);
        // not optimal
        // take whole object from db and load it to memory
        // can be used if method will return deleted object
        //Return bool: Use DeleteOneAsync (Fastest).
        //Return Order: Use FindOneAndDeleteAsync (Slower, but gives data).
        // var result = await _orders.FindOneAndDeleteAsync(filter);

        var result = await _orders.DeleteOneAsync(filter);

        return result.DeletedCount > 0;
    }
}