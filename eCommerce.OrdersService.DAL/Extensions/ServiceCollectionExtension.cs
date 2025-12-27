using eCommerce.OrdersService.DAL.Repositories;
using eCommerce.OrdersService.DAL.RepositoryInterfases;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace eCommerce.OrdersService.DAL.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
    {
        try
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
        }
        catch (BsonSerializationException ex)
        {
            Console.WriteLine(ex.Message);
        }

        // 2. Connection String Logic
        string connectionStringTemplate = configuration.GetConnectionString("DefaultConnection")!;
        var connectionString = connectionStringTemplate
            .Replace("$MONGODB_HOST",configuration["MONGODB_HOST"] ?? "localhost")
            .Replace("$MONGODB_PORT", configuration["MONGODB_PORT"] ?? "27018");

        var mongoClient = new MongoClient(connectionString);
        services.AddSingleton<IMongoClient>(mongoClient);

        // 3. Register Database
        services.AddScoped<IMongoDatabase>(provider =>
        {
            var dbName = configuration["MongoDbSettings:DatabaseName"];
            if (string.IsNullOrEmpty(dbName))
            {
                dbName = "OrdersDatabase"; // Matches init.js
            }

            return mongoClient.GetDatabase(dbName);
        });

        services.AddScoped<IOrdersRepository, OrdersRepository>();

        return services;
    }
}