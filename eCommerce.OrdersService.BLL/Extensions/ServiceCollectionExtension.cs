using eCommerce.OrdersService.BLL.DTOs;
using eCommerce.OrdersService.BLL.HttpClients;
using eCommerce.OrdersService.BLL.Mappers;
using eCommerce.OrdersService.BLL.ServicesInterfaces;
using eCommerce.OrdersService.BLL.Validators;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
                        $"https://{configuration["UsersMicroserviceName"]}:{configuration["UsersMicroservicePort"]}/");
            });

        return services;
    }
}