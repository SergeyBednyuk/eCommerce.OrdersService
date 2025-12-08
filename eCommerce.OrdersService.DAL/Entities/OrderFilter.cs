namespace eCommerce.OrdersService.DAL.Entities;

public record OrderFilter
{
    public Guid? OrderId { get; init; }
    public Guid? UserId { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public decimal? MinTotal { get; init; }
    public decimal? MaxTotal { get; init; }
}