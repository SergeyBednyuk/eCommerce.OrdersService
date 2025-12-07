namespace eCommerce.OrdersService.BLL.DTOs;

public record OrderItemUpdateRequest(Guid ProductId, decimal UnitPrice, int Quantity)
{
    
}