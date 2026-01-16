using eCommerce.OrdersService.DAL.Entities;
using eCommerce.OrdersService.DAL.RepositoryInterfases;
using MongoDB.Bson;
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

    public async Task<IEnumerable<Order>> GetOrdersByConditionAsync(OrderFilter filterDto)
    {
        var filter = BuildFilter(filterDto);

        var result = await _orders.Find(filter)
            .SortByDescending(x => x.OrderDate)
            .Skip((filterDto.PageNumber - 1) * filterDto.PageSize)
            .Limit(filterDto.PageSize)
            .ToListAsync();
        var f1 = Builders<Order>.Filter.Empty;
        
        return result;
    }

    public async Task<Order?> CreateAsync(Order order)
    {
        order.OrderId = Guid.NewGuid();
        order.Id = order.OrderId;

        foreach (var orderItem in order.OrderItems)
        {
            orderItem._id =  Guid.NewGuid();
        }
        
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

    private FilterDefinition<Order> BuildFilter(OrderFilter filterDto)
    {
        var builder = Builders<Order>.Filter;
        var filter = builder.Empty;

        if (filterDto.OrderId.HasValue) filter &= builder.Eq(x => x.OrderId, filterDto.OrderId.Value);
        if (filterDto.UserId.HasValue) filter &= builder.Eq(x => x.UserId, filterDto.UserId.Value);
        // Date Range
        if (filterDto.FromDate.HasValue) filter &= builder.Gte(x => x.OrderDate, filterDto.FromDate.Value);
        if (filterDto.ToDate.HasValue) filter &= builder.Lte(x => x.OrderDate, filterDto.ToDate.Value);
        // Price Range: Check Min and Max independently
        if (filterDto.MinTotal.HasValue) filter &= builder.Gte(x => x.Total, filterDto.MinTotal.Value);
        if (filterDto.MaxTotal.HasValue) filter &= builder.Lte(x => x.Total, filterDto.MaxTotal.Value);

        return filter;
    }
}