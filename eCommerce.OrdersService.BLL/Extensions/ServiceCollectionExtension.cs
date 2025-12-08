using eCommerce.OrdersService.BLL.DTOs;
using eCommerce.OrdersService.BLL.Mappers;
using eCommerce.OrdersService.BLL.ServicesInterfaces;
using eCommerce.OrdersService.BLL.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace eCommerce.OrdersService.BLL.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddBusinesLogicLayer(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => { }, typeof(OrderMappingProfile));
        
        services.AddScoped<IValidator<AddOrderRequest>, AddOrderRequestValidator>();
        services.AddScoped<IValidator<UpdateOrderRequest>, UpdateOrderRequestValidator>();
        services.AddScoped<IValidator<OrderItemRequest>, OrderItemRequestValidator>();
        
        services.AddScoped<IOrdersService, Services.OrdersService>();
        
        return services;
    }
}