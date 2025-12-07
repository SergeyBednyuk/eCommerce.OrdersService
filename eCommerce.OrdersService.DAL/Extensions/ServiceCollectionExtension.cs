using eCommerce.OrdersService.DAL.Repositories;
using eCommerce.OrdersService.DAL.RepositoryInterfases;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace eCommerce.OrdersService.DAL.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionStringTemplate = configuration.GetConnectionString("ConnectionStrings")!;
        var connectionString = connectionStringTemplate
            .Replace("$MONGODB_HOST", Environment.GetEnvironmentVariable("MONGODB_HOST") ?? "localhost")
            .Replace("$MONGODB_PORT", Environment.GetEnvironmentVariable("MONGODB_PORT") ?? "27017");

        services.AddSingleton<IMongoClient>(new MongoClient(connectionString));
        services.AddScoped<IMongoDatabase>(provider =>
        {
            var client = provider.GetRequiredService<IMongoClient>();
            return client.GetDatabase("eCommerce.Orders");
        });
        
        // 4. Register Repositories
        services.AddScoped<IOrdersRepository, OrdersRepository>();

        return services;
    }
}