namespace eCommerce.OrdersService.BLL.DTOs;

public record UpdateStockRequest(IEnumerable<UpdateStockDto> UpdateStockItems, bool Reduce = true);