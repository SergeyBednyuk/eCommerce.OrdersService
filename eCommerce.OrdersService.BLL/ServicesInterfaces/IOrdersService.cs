using eCommerce.OrdersService.BLL.DTOs;

namespace eCommerce.OrdersService.BLL.ServicesInterfaces;

public interface IOrdersService
{
    Task<OrderResponse<IEnumerable<OrderDto>>> GetOrdersAsync(int page, int pageSize);
    Task<OrderResponse<OrderDto>> GetOrderAsync(Guid orderId);
    Task<OrderResponse<IEnumerable<OrderDto>>> GetOrdersByCondition(OrderFilterDto filter);
    Task<OrderResponse<OrderDto>> GetOrderByConditionAsync(OrderFilterDto filter);
    Task<OrderResponse<OrderDto>> CreateOrderAsync(AddOrderRequest addOrderRequest);
    Task<OrderResponse<OrderDto>> UpdateOrderAsync(UpdateOrderRequest updateOrderRequest);
    Task<OrderResponse<OrderDto>> DeleteOrderAsync(Guid orderId);
}