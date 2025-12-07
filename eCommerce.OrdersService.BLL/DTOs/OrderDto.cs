namespace eCommerce.OrdersService.BLL.DTOs;

public class OrderDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal Total { get; set; }
    public IEnumerable<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
}