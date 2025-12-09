namespace eCommerce.OrdersService.BLL.DTOs;

public record GetOrdersQuery
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public Guid? OrderId { get; init; } 
    public Guid? UserId { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public decimal? MinTotal { get; init; }
    public decimal? MaxTotal { get; init; }
}