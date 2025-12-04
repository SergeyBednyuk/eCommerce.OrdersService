using eCommerce.OrdersService.DAL.Entities;
using eCommerce.OrdersService.DAL.Settings;
using MongoDB.Driver;

namespace eCommerce.OrdersService.DAL.Context;

public class ApplicationDbContext
{
    private readonly IMongoDatabase _database;

    public ApplicationDbContext(MongoDbSettings settings)
    {
        var mongoDbClient = new MongoClient(settings.ConnectionString);
        _database = mongoDbClient.GetDatabase(settings.DataBaseName);
    }

    public IMongoCollection<Order> Orders => _database.GetCollection<Order>("Orders");
}