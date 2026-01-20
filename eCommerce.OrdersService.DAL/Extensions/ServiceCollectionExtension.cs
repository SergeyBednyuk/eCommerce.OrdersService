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

        var host = configuration["MONGODB_HOST"] ?? "localhost";
        var port = configuration["MONGODB_PORT"] ?? "27017";
        var dbName = configuration["MongoDbSettings:DatabaseName"];
        if (string.IsNullOrEmpty(dbName)) dbName = "OrdersDatabase";
        
        var connectionString = $"mongodb://root:example@{host}:{port}/?authSource={dbName}";

        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (environment == "Development")
        {
            Console.WriteLine($"[MongoDB] Connecting to: {host}:{port}, AuthSource: {dbName}");
        }

        var mongoClient = new MongoClient(connectionString);
        services.AddSingleton<IMongoClient>(mongoClient);
        services.AddScoped<IMongoDatabase>(provider => mongoClient.GetDatabase(dbName));

        services.AddScoped<IOrdersRepository, OrdersRepository>();

        return services;
    }
}