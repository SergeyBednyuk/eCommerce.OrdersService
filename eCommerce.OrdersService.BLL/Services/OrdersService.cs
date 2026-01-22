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

        var orderFromDb = await _ordersRepository.GetByIdAsync(orderId);
        if (orderFromDb is null)
        {
            _logger.LogInformation("Order with id {orderId} not found", orderId);
            return OrderResponse<OrderDto>.Failure(null, $"there is no order with {orderId} id");
        }

        var orderDto = _mapper.Map<OrderDto>(orderFromDb);

        // get all product ids
        var productIds = orderDto.OrderItems.Select(x => x.ProductId).Distinct();

        //get products by ids
        var productsClientResult = await _productsMicroserviceClient.GetProductsByIdsAsync(productIds);

        if (productsClientResult.IsSuccess && productsClientResult.Data is not null)
        {
            var products = productsClientResult.Data.ToDictionary(p => p.Id, p => p);
            foreach (var itemDto in orderDto.OrderItems)
            {
                if (products.TryGetValue(itemDto.ProductId, out var product))
                {
                    itemDto.ProductName = product.Name;
                }
            }
        }

        return OrderResponse<OrderDto>.Success(orderDto);
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
        var productsResult = await _ordersRepository.GetOrdersByConditionAsync(mappedFilter);
        if (!productsResult.Any())
        {
            _logger.LogInformation("No orders found for page {pageNumber}", query.PageNumber);
            return
                OrderResponse<IEnumerable<OrderDto>>.Failure(data: null,
                    message: $"there are no orders in {query.PageNumber} page and {query.PageSize} page size range");
        }

        var mappedResult = _mapper.Map<IEnumerable<OrderDto>>(productsResult).ToList();

        var allProductsIds = mappedResult.SelectMany(x => x.OrderItems)
            .Select(x => x.ProductId)
            .Distinct();

        var productsClientResult = await _productsMicroserviceClient.GetProductsByIdsAsync(allProductsIds);

        if (productsClientResult.IsSuccess && productsClientResult.Data is not null)
        {
            var products = productsClientResult.Data.ToDictionary(x => x.Id, x => x.Name);
            foreach (var orderDto in mappedResult)
            {
                foreach (var orderItemDto in orderDto.OrderItems)
                {
                    if (products.TryGetValue(orderItemDto.ProductId, out var product))
                    {
                        orderItemDto.ProductName = product;
                    }
                }
            }
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

        var mappedOrderItems = _mapper.Map<List<UpdateStockDto>>(addOrderRequest.OrderItems);

        var updateStockRequest =
            new UpdateStockRequest(mappedOrderItems, Reduce: true);


        var reduceProductStockResult =
            await _productsMicroserviceClient.UpdateProductStockByIdAsync(updateStockRequest);

        if (reduceProductStockResult.IsSuccess)
        {
            var mappedOrder = _mapper.Map<Order>(addOrderRequest);

            mappedOrder.Total = mappedOrder.OrderItems.Sum(x => x.TotalPrice);

            var result = await _ordersRepository.CreateAsync(mappedOrder);
            if (result is null)
            {
                _logger.LogInformation("Order creation failed");

                updateStockRequest =
                    new UpdateStockRequest(mappedOrderItems, Reduce: false);
                reduceProductStockResult =
                    await _productsMicroserviceClient.UpdateProductStockByIdAsync(updateStockRequest);

                return OrderResponse<OrderDto>.Failure(null, "Order creation failed");
            }

            return OrderResponse<OrderDto>.Success(_mapper.Map<OrderDto>(result));
        }
        else
        {
            var errors = string.Join(", ", reduceProductStockResult?.Errors ??
                                           ["Reduce Product Stock request was failed"]);
            return OrderResponse<OrderDto>.Failure(null,
                $"Order creation failed because: {errors}");
        }
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
            return OrderResponse<OrderDto>.Failure(null, $"Order with {updateOrderRequest.OrderId} id is not found");
        }

        var reduceOrderItemsStock = new List<UpdateStockDto>();
        var increaseOrderItemsStock = new List<UpdateStockDto>();

        var oldItems = existedOrder.OrderItems.ToDictionary(x => x.ProductId, x => x.Quantity);
        var newItems = updateOrderRequest.OrderItems.ToDictionary(x => x.ProductId, x => x.Quantity);

        foreach (var newItem in newItems)
        {
            if (oldItems.TryGetValue(newItem.Key, out var oldQnt))
            {
                if (newItem.Value > oldQnt)
                {
                    reduceOrderItemsStock.Add(new UpdateStockDto(newItem.Key, newItem.Value - oldQnt));
                }
                else if (newItem.Value < oldQnt)
                {
                    increaseOrderItemsStock.Add(new UpdateStockDto(newItem.Key, oldQnt - newItem.Value));
                }
            }
            else
            {
                reduceOrderItemsStock.Add(new UpdateStockDto(newItem.Key, newItem.Value));
            }
        }

        foreach (var oldItem in oldItems)
        {
            if (!newItems.ContainsKey(oldItem.Key))
            {
                increaseOrderItemsStock.Add(new UpdateStockDto(oldItem.Key, oldItem.Value));
            }
        }

        if (reduceOrderItemsStock.Any())
        {
            var reduceResult = await _productsMicroserviceClient.UpdateProductStockByIdAsync(
                new UpdateStockRequest(reduceOrderItemsStock, Reduce: true));

            if (!reduceResult.IsSuccess)
            {
                return OrderResponse<OrderDto>.Failure(null,
                    $"Update failed. Insufficient stock: {reduceResult.Message}", reduceResult.Errors);
            }
        }

        _mapper.Map(updateOrderRequest, existedOrder);
        existedOrder.Total = existedOrder.OrderItems.Sum(x => x.TotalPrice);

        var dbResult = await _ordersRepository.UpdateAsync(existedOrder);

        if (dbResult is null)
        {
            _logger.LogError("Database update failed for Order {OrderId}. Initiating Rollback.",
                updateOrderRequest.OrderId);

            if (reduceOrderItemsStock.Any())
            {
                var rollbackResult = await _productsMicroserviceClient.UpdateProductStockByIdAsync(
                    new UpdateStockRequest(reduceOrderItemsStock, Reduce: false)); // Reduce=false means Increase

                if (!rollbackResult.IsSuccess)
                {
                    _logger.LogCritical(
                        "CRITICAL: DATA INCONSISTENCY. Failed to rollback stock for Order {OrderId}. Products: {Products}",
                        updateOrderRequest.OrderId, string.Join(", ", reduceOrderItemsStock.Select(x => x.Id)));
                }
            }

            return OrderResponse<OrderDto>.Failure(null, "Order update failed due to database error");
        }

        if (increaseOrderItemsStock.Any())
        {
            var releaseResult = await _productsMicroserviceClient.UpdateProductStockByIdAsync(
                new UpdateStockRequest(increaseOrderItemsStock, Reduce: false));

            if (!releaseResult.IsSuccess)
            {
                _logger.LogError(
                    "Stock release failed for Order {OrderId}. Inventory counts may be higher than actual. Error: {Error}",
                    updateOrderRequest.OrderId, releaseResult.Message);
            }
        }

        return OrderResponse<OrderDto>.Success(_mapper.Map<OrderDto>(dbResult));
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