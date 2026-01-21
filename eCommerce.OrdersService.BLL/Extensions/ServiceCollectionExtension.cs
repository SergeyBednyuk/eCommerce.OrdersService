using eCommerce.OrdersService.BLL.DTOs;
using eCommerce.OrdersService.BLL.HttpClients;
using eCommerce.OrdersService.BLL.Mappers;
using eCommerce.OrdersService.BLL.Policies;
using eCommerce.OrdersService.BLL.ServicesInterfaces;
using eCommerce.OrdersService.BLL.Validators;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;

namespace eCommerce.OrdersService.BLL.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddBusinesLogicLayer(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAutoMapper(cfg => { }, typeof(OrderMappingProfile));

        services.AddScoped<IValidator<AddOrderRequest>, AddOrderRequestValidator>();
        services.AddScoped<IValidator<UpdateOrderRequest>, UpdateOrderRequestValidator>();
        services.AddScoped<IValidator<OrderItemRequest>, OrderItemRequestValidator>();
        services.AddScoped<IValidator<GetOrdersQuery>, GetOrdersQueryValidator>();

        services.AddScoped<IOrdersService, Services.OrdersService>();

        services.AddHttpClient<UsersMicroserviceClient>("UsersApi",
                client =>
                {
                    var host = configuration["UsersMicroserviceName"] ?? "localhost";
                    var port = configuration["UsersMicroservicePort"] ?? "5129"; // Default local port
                    client.BaseAddress = new Uri($"http://{host}:{port}/");
                })
                .AddPolicyHandler((provider, request) =>
                {
                    var logger = provider.GetRequiredService<ILogger<UsersMicroserviceClient>>();
                    return ResiliencyPolicies.GetBulkheadPolicy(logger, 5, 10);
                })
                .AddPolicyHandler((provider, request) =>
                {
                    var logger = provider.GetRequiredService<ILogger<UsersMicroserviceClient>>();
                    return ResiliencyPolicies.GetRetryPolicy(logger);
                })
                .AddPolicyHandler((provider, request) =>
                {
                    var logger = provider.GetRequiredService<ILogger<UsersMicroserviceClient>>();
                    return ResiliencyPolicies.GetCircuitBreakerPolicy(logger);
                })
                .AddPolicyHandler(ResiliencyPolicies.GetTimeoutPolicy());

        services.AddHttpClient<ProductsMicroserviceClient>("ProductsApi",
                client =>
                {
                    var host = configuration["ProductsServiceHost"] ?? "localhost";
                    var port = configuration["ProductsServicePort"] ?? "5173"; // Default local port
                    client.BaseAddress = new Uri($"http://{host}:{port}/");
                })
                .AddPolicyHandler((provider, request) =>
                {
                    var logger = provider.GetRequiredService<ILogger<ProductsMicroserviceClient>>();
                    return ResiliencyPolicies.GetBulkheadPolicy(logger, maxParallelization: 10, maxQueuingActions: 15);
                })
                .AddPolicyHandler((provider, request) =>
                {
                    var logger = provider.GetRequiredService<ILogger<ProductsMicroserviceClient>>();
                    return ResiliencyPolicies.GetRetryPolicy(logger);
                })
                .AddPolicyHandler((provider, request) =>
                {
                    var logger = provider.GetRequiredService<ILogger<ProductsMicroserviceClient>>();
                    return ResiliencyPolicies.GetCircuitBreakerPolicy(logger);
                })
                .AddPolicyHandler(ResiliencyPolicies.GetTimeoutPolicy());


        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = $"{configuration["REDIS_HOST"]}:{configuration["REDIS_PORT"]}";
        });

        return services;
    }
}