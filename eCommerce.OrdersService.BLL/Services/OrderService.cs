using AutoMapper;
using eCommerce.OrdersService.BLL.DTOs;
using eCommerce.OrdersService.BLL.ServicesInterfases;
using eCommerce.OrdersService.DAL.Entities;
using eCommerce.OrdersService.DAL.RepositoryInterfases;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace eCommerce.OrdersService.BLL.Services;

public class OrderService(IOrdersRepository ordersRepository, IMapper mapper, ILogger<OrderService> logger) : IOrdersService
{
    private readonly IOrdersRepository _ordersRepository = ordersRepository;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<OrderService> _logger = logger;


    public Task<OrderResponse<IEnumerable<OrderDto>>> GetOrdersAsync(int page, int pageSize)
    {
        throw new NotImplementedException();
    }

    public Task<OrderResponse<OrderDto?>> GetOrderAsync(Guid orderId)
    {
        throw new NotImplementedException();
    }

    public Task<OrderResponse<IEnumerable<OrderDto>>> GetOrdersByCondition(FilterDefinition<Order> filter)
    {
        throw new NotImplementedException();
    }

    public Task<OrderResponse<OrderDto?>> GetOrderByConditionAsync(FilterDefinition<Order> filter)
    {
        throw new NotImplementedException();
    }

    public Task<OrderResponse<OrderDto?>> CreateOrderAsync(AddOrderRequest addOrderRequest)
    {
        throw new NotImplementedException();
    }

    public Task<OrderResponse<OrderDto?>> UpdateOrderAsync(UpdateOrderRequest updateOrderRequest)
    {
        throw new NotImplementedException();
    }

    public Task<OrderResponse<OrderDto?>> DeleteOrderAsync(Guid orderId)
    {
        throw new NotImplementedException();
    }
}