using eCommerce.OrdersService.BLL.DTOs;
using eCommerce.OrdersService.BLL.ServicesInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.OrdersService.API.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController(IOrdersService ordersService, ILogger<OrdersController> logger) : ControllerBase
{
    private readonly IOrdersService _ordersService = ordersService;
    private readonly ILogger<OrdersController> _logger = logger;

    [HttpGet]
    public async Task<IActionResult> GetOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        _logger.LogInformation("Retrieving {PageSize} orders from page {Page}", pageSize, page);

        var result = await _ordersService.GetOrdersAsync(page, pageSize);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("there is not any orders for this page");
            return NotFound(result);
        }

        return Ok(result);
    }

    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> GetOrder(Guid id)
    {
        _logger.LogInformation("Retrieving order details for ID: {OrderId}", id);

        var result = await _ordersService.GetOrderAsync(id);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Order with ID: {OrderId} not found", id);
            return NotFound(result);
        }

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetOrdersByOrderDate([FromQuery] DateTime orderDate)
    {
        _logger.LogInformation("Retrieving orders for {orderDate} date", orderDate);

        var result =
            await _ordersService.GetOrdersByCondition(new OrderFilterDto(null, null, orderDate, orderDate, null, null));
        
        if (!result.IsSuccess)
        {
            _logger.LogWarning("There is not any orders for this date");
            return NotFound(result);
        }

        return Ok(result);
    }
    
    
}