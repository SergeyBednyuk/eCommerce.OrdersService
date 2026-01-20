using eCommerce.OrdersService.BLL.DTOs;
using eCommerce.OrdersService.BLL.HttpClients;
using eCommerce.OrdersService.BLL.Mappers;
using eCommerce.OrdersService.BLL.ServicesInterfaces;
using eCommerce.OrdersService.BLL.Validators;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
                //TODO base option if there are no variable
                client.BaseAddress =
                    new Uri(
                        $"http://{configuration["UsersMicroserviceName"]}:{configuration["UsersMicroservicePort"]}/");
            }).AddPolicyHandler(
            Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .WaitAndRetryAsync(
                    retryCount: 2,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(1),
                    //TO DO
                    onRetry: (result, span, retryAttempt, context) => { }
                )
        );

        services.AddHttpClient<ProductsMicroserviceClient>("ProductsApi",
            client =>
            {
                client.BaseAddress =
                    new Uri(
                        $"http://{configuration["ProductsServiceHost"]}:{configuration["ProductsServicePort"]}/");
            }).AddPolicyHandler(
            Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .WaitAndRetryAsync(
                    retryCount: 2,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(1),
                    //TO DO
                    onRetry: (result, span, retryAttempt, context) => { }
                )
        );

        return services;
    }
}