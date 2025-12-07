namespace eCommerce.OrdersService.BLL.DTOs;

public class UpdateOrderRequest(Guid OrderId ,Guid UserId, DateTime OrderDate, List<OrderItemRequest> OrderItems)
{
    public UpdateOrderRequest() : this(default, default, default, default)
    {
        
    }
}