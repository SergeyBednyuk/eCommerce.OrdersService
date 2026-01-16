namespace eCommerce.OrdersService.BLL.DTOs;

public record UpdateOrderRequest(Guid OrderId ,Guid UserId, DateTime OrderDate, IEnumerable<OrderItemRequest> OrderItems)
{
    public UpdateOrderRequest() : this(default, default, default, default)
    {
        
    }
}