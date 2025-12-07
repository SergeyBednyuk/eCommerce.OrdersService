using eCommerce.OrdersService.BLL.DTOs;
using eCommerce.OrdersService.DAL.Entities;
using MongoDB.Driver;

namespace eCommerce.OrdersService.BLL.ServicesInterfases;

public interface IOrdersService
{
    Task<OrderResponse<IEnumerable<OrderDto>>> GetOrdersAsync(int page, int pageSize);
    Task<OrderResponse<OrderDto?>> GetOrderAsync(Guid orderId);
    Task<OrderResponse<IEnumerable<OrderDto>>> GetOrdersByCondition(FilterDefinition<Order> filter);
    Task<OrderResponse<OrderDto?>> GetOrderByConditionAsync(FilterDefinition<Order> filter);
    Task<OrderResponse<OrderDto?>> CreateOrderAsync(AddOrderRequest addOrderRequest);
    Task<OrderResponse<OrderDto?>> UpdateOrderAsync(UpdateOrderRequest updateOrderRequest);
    Task<OrderResponse<OrderDto?>> DeleteOrderAsync(Guid orderId);
}