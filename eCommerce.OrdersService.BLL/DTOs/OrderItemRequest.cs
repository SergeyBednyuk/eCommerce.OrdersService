namespace eCommerce.OrdersService.BLL.DTOs;

public record OrderItemRequest(Guid ProductId, decimal UnitPrice, int Quantity)
{
    public OrderItemRequest() : this(default, default, default)
    {
    }
}