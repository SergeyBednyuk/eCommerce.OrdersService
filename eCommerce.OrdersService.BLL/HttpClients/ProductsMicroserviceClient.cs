using System.Net.Http.Json;
using System.Text.Json;
using eCommerce.OrdersService.BLL.DTOs;
using Microsoft.Extensions.Logging;

namespace eCommerce.OrdersService.BLL.HttpClients;

public class ProductsMicroserviceClient(HttpClient httpClient, ILogger<ProductsMicroserviceClient> logger)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<ProductsMicroserviceClient> _logger = logger;

    public async Task<AppResponse<IEnumerable<ProductDTO>>> GetProductsByIdsAsync(IEnumerable<Guid> ids)
    {
        try
        {
            string url = $"api/products/search/batch";
            var payload = new 
            { 
                Ids = ids, 
            };
            
            var response = await _httpClient.PutAsJsonAsync(url, payload);
            
            var result = await response.Content.ReadFromJsonAsync<AppResponse<IEnumerable<ProductDTO>>>(new JsonSerializerOptions() { PropertyNameCaseInsensitive = true});

            if (result is not null && result.IsSuccess)
            {
                return result;
            }
            
            _logger.LogWarning($"Failed to get products for {ids.Select(x => x.ToString())} ids from Product Service");
            return AppResponse<IEnumerable<ProductDTO>>.Failure(null, result.Message, result.Errors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Network error communicating with Product Service for {ids.Select(x => x.ToString())}");
            return AppResponse<IEnumerable<ProductDTO>>.Failure(null, $"Network error communicating with Product Service for {ids.Select(x => x.ToString())}");
        }
    }
    
    public async Task<AppResponse<ProductDTO>> UpdateProductStockByIdAsync(Guid productId, int quantity, bool reduce = true)
    {
        try
        {
            string url = $"api/products/{productId}/reduce-stock";
            
            var payload = new 
            { 
                Quantity = quantity, 
                Reduce = reduce 
            };
            
            var response = await _httpClient.PutAsJsonAsync(url, payload);
            
            var result = await response.Content.ReadFromJsonAsync<AppResponse<ProductDTO>>(new JsonSerializerOptions() { PropertyNameCaseInsensitive = true});

            if (result is not null && result.IsSuccess)
            {
                return result;
            }
            
            _logger.LogWarning("Failed to adjust stock for {ProductId}. Reduce: {Reduce}. Errors: {Errors}. Message: {Message}", 
                productId, reduce, result.Errors, result.Message);
            
            return AppResponse<ProductDTO>.Failure(null, result.Message, result.Errors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Network error communicating with Product Service for {ProductId}", productId);
            return AppResponse<ProductDTO>.Failure(null, $"Network error communicating with Product Service for {productId}");
        }
    }
}