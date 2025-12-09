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
    public async Task<IActionResult> GetOrders([FromQuery] GetOrdersQuery query)
    {
        _logger.LogInformation("Retrieving orders from {getOrdersQuery}", query);

        var result = await _ordersService.GetOrdersAsync(query);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("there is not any orders for this page");
            return BadRequest(result);
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

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] AddOrderRequest createOrderRequest)
    {
        _logger.LogInformation("Processing create order request for {createOrder}", createOrderRequest);

        var result = await _ordersService.CreateOrderAsync(createOrderRequest);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("New order cannot be created");
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(GetOrder), new { id = result.Data!.Id }, result);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateOrder([FromBody] UpdateOrderRequest updateOrderRequest)
    {
        _logger.LogInformation("Processing update order request for {updateOrder}", updateOrderRequest);

        var result = await _ordersService.UpdateOrderAsync(updateOrderRequest);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("The order cannot be updated");
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpDelete]
    [Route("{id:guid}")]
    public async Task<IActionResult> DeleteOrder(Guid id)
    {
        _logger.LogInformation("Processing delete order request for {orderId} id", id);

        var result = await _ordersService.DeleteOrderAsync(id);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("The order cannot be deleted");
            return NotFound(result);
        }

        return Ok(result);
    }
}