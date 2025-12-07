namespace eCommerce.OrdersService.BLL.DTOs;

public class OrderItemDto(Guid ProductId, decimal UnitPrice, int Quantity, decimal TotalPrice)
{
    public Guid ProductId { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
}