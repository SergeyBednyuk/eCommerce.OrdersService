namespace eCommerce.OrdersService.BLL.DTOs;

public record UpdateStockDto(Guid Id, int Quantity)
{
    public UpdateStockDto() : this(default, default)
    {
        
    }
}