namespace eCommerce.OrdersService.BLL.DTOs;

public class ProductDTO
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Category { get; set; }
    public decimal UnitPrice { get; set; }
    public int QuantityInStock { get; set; }
}