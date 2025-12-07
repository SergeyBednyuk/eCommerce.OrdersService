namespace eCommerce.OrdersService.BLL.DTOs;

public record AddOrderRequest(Guid UserId, DateTime OrderDate, List<OrderItemRequest> OrderItems)
{
    public AddOrderRequest() : this(default, default, default)
    {
        
    }
}