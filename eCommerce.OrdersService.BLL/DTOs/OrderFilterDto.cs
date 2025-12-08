namespace eCommerce.OrdersService.BLL.DTOs;

public record OrderFilterDto(
    Guid? OrderId,
    Guid? UserId,
    DateTime? FromDate,
    DateTime? ToDate,
    decimal? MinTotal,
    decimal? MaxTotal);