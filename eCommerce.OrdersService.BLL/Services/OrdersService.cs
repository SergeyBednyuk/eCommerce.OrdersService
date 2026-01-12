using AutoMapper;
using eCommerce.OrdersService.BLL.DTOs;
using eCommerce.OrdersService.BLL.HttpClients;
using eCommerce.OrdersService.BLL.ServicesInterfaces;
using eCommerce.OrdersService.DAL.Entities;
using eCommerce.OrdersService.DAL.RepositoryInterfases;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace eCommerce.OrdersService.BLL.Services;

public class OrdersService(
    IOrdersRepository ordersRepository,
    IMapper mapper,
    ILogger<OrdersService> logger,
    IValidator<GetOrdersQuery> getOrdersQueryValidator,
    IValidator<AddOrderRequest> addOrderRequestValidator,
    IValidator<UpdateOrderRequest> updateOrderRequestValidator, 
    UsersMicroserviceClient usersMicroserviceClient,
    ProductsMicroserviceClient productsMicroserviceClient) : IOrdersService
{
    private readonly IOrdersRepository _ordersRepository = ordersRepository;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<OrdersService> _logger = logger;
    private readonly UsersMicroserviceClient _usersMicroserviceClient = usersMicroserviceClient;
    private readonly ProductsMicroserviceClient _productsMicroserviceClient = productsMicroserviceClient;

    //Validators
    private readonly IValidator<GetOrdersQuery> _getOrdersQueryValidator = getOrdersQueryValidator;
    private readonly IValidator<AddOrderRequest> _addOrderRequestValidator = addOrderRequestValidator;
    private readonly IValidator<UpdateOrderRequest> _updateOrderRequestValidator = updateOrderRequestValidator;

    public async Task<OrderResponse<OrderDto>> GetOrderAsync(Guid orderId)
    {
        _logger.LogInformation("Getting order with id {orderId}...");
        var result = await _ordersRepository.GetByIdAsync(orderId);
        if (result is null)
        {
            _logger.LogInformation("Order with id {orderId} not found", orderId);
            return OrderResponse<OrderDto>.Failure(null, $"there is no order with {orderId} id");
        }

        return OrderResponse<OrderDto>.Success(_mapper.Map<OrderDto>(result));
    }

    public async Task<OrderResponse<IEnumerable<OrderDto>>> GetOrdersAsync(GetOrdersQuery query)
    {
        _logger.LogInformation("Getting orders with {OrdersQuery} query", query);

        var validationResult = await _getOrdersQueryValidator.ValidateAsync(query);
        if (!validationResult.IsValid)
        {
            return OrderResponse<IEnumerable<OrderDto>>.Failure(null, $"validation failed of {typeof(GetOrdersQuery)}",
                validationResult.Errors.Select(x => x.ErrorMessage));
        }

        var mappedFilter = _mapper.Map<OrderFilter>(query);
        var result = await _ordersRepository.GetOrdersByConditionAsync(mappedFilter);
        var mappedResult = _mapper.Map<IEnumerable<OrderDto>>(result);
        if (!result.Any())
        {
            _logger.LogInformation("No orders found for page {pageNumber}", query.PageNumber);
            return
                OrderResponse<IEnumerable<OrderDto>>.Success(data: mappedResult,
                    message: $"there are no orders in {query.PageNumber} page and {query.PageSize} page size range");
        }

        return OrderResponse<IEnumerable<OrderDto>>.Success(mappedResult);
    }


    public async Task<OrderResponse<OrderDto>> CreateOrderAsync(AddOrderRequest addOrderRequest)
    {
        _logger.LogInformation("adding new order");
        
        var validationResult = await _addOrderRequestValidator.ValidateAsync(addOrderRequest);
        if (!validationResult.IsValid)
        {
            _logger.LogError("Order validation failed");
            return OrderResponse<OrderDto>.Failure(null, "Validation failed",
                validationResult.Errors.Select(x => x.ErrorMessage));
        }
        
        var user = await _usersMicroserviceClient.GetUserByIdAsync(addOrderRequest.UserId);
        if (!user.IsSuccess)
        {
            return OrderResponse<OrderDto>.Failure(null, user.Message, user.Errors);
        }
        
        for
        
        var reduceProductStockResult = await _productsMicroserviceClient.UpdateProductStockByIdAsync()
        
        var mappedOrder = _mapper.Map<Order>(addOrderRequest);

        mappedOrder.Total = mappedOrder.OrderItems.Sum(x => x.TotalPrice);

        var result = await _ordersRepository.CreateAsync(mappedOrder);
        if (result is null)
        {
            _logger.LogInformation("Order creation failed");
            return OrderResponse<OrderDto>.Failure(null, "Order creation failed");
        }

        return OrderResponse<OrderDto>.Success(_mapper.Map<OrderDto>(result));
    }

    public async Task<OrderResponse<OrderDto>> UpdateOrderAsync(UpdateOrderRequest updateOrderRequest)
    {
        _logger.LogInformation("Updating order {OrderId}", updateOrderRequest.OrderId);

        var validationResult = await _updateOrderRequestValidator.ValidateAsync(updateOrderRequest);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Order validation failed for {OrderId}", updateOrderRequest.OrderId);
            return OrderResponse<OrderDto>.Failure(null, "Validation failed",
                validationResult.Errors.Select(x => x.ErrorMessage));
        }
        
        var user = await _usersMicroserviceClient.GetUserByIdAsync(updateOrderRequest.UserId);
        if (!user.IsSuccess)
        {
            return OrderResponse<OrderDto>.Failure(null, user.Message, user.Errors);
        }

        var existedOrder = await _ordersRepository.GetByIdAsync(updateOrderRequest.OrderId);

        if (existedOrder is null)
        {
            _logger.LogWarning("Order {OrderId} not found during update", updateOrderRequest.OrderId);
            // Fixed: Added $ for interpolation
            return OrderResponse<OrderDto>.Failure(null, $"Order with {updateOrderRequest.OrderId} id is not found");
        }

        _mapper.Map(updateOrderRequest, existedOrder);

        var result = await _ordersRepository.UpdateAsync(existedOrder);

        if (result is null)
        {
            _logger.LogError("Order update failed for {OrderId}", updateOrderRequest.OrderId);
            return OrderResponse<OrderDto>.Failure(null, "Order update failed");
        }
        
        result.Total = result.OrderItems.Sum(x => x.TotalPrice);
        
        return OrderResponse<OrderDto>.Success(_mapper.Map<OrderDto>(result));
    }

    public async Task<OrderResponse<OrderDto>> DeleteOrderAsync(Guid orderId)
    {
        _logger.LogInformation("Deleting order with id {orderId}", orderId);
        var result = await _ordersRepository.DeleteAsync(orderId);
        if (!result)
        {
            _logger.LogInformation("Order with id {orderId} not found", orderId);
            return OrderResponse<OrderDto>.Failure(null, $"there is no order with {orderId} id");
        }

        return OrderResponse<OrderDto>.Success(null, "Order deleted successfully");
    }
}