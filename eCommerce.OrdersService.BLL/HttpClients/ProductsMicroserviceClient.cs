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

            var result =
                await response.Content.ReadFromJsonAsync<AppResponse<IEnumerable<ProductDTO>>>(
                    new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            if (result is not null && result.IsSuccess)
            {
                return result;
            }

            _logger.LogWarning($"Failed to get products for {ids.Select(x => x.ToString())} ids from Product Service");
            return AppResponse<IEnumerable<ProductDTO>>.Failure(null, result.Message, result.Errors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                $"Network error communicating with Product Service for {ids.Select(x => x.ToString())}");
            return AppResponse<IEnumerable<ProductDTO>>.Failure(null,
                $"Network error communicating with Product Service for {ids.Select(x => x.ToString())}");
        }
    }

    public async Task<AppResponse<IEnumerable<ProductDTO>>> UpdateProductStockByIdAsync(UpdateStockRequest request)
    {
        try
        {
            string url = $"api/products/stock/batch-update";

            var response = await _httpClient.PutAsJsonAsync(url, request);

            var result = await response.Content.ReadFromJsonAsync<AppResponse<IEnumerable<ProductDTO>>>(new JsonSerializerOptions()
                { PropertyNameCaseInsensitive = true });

            if (result is not null && result.IsSuccess)
            {
                return result;
            }

            _logger.LogWarning("Failed to adjust stock thought Orders API. Errore: {errors} message {message} ",
                result.Errors, result.Message);

            return AppResponse<IEnumerable<ProductDTO>>.Failure(null, result.Message, result.Errors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Network error communicating with Product Service for {ProductIds}",
                string.Join(", ", request.UpdateStockItems.Select(x => x.Id)));
            return AppResponse<IEnumerable<ProductDTO>>.Failure(null,
                $"Network error communicating with Product Service for {request.UpdateStockItems.Select(x => x.Id)}");
        }
    }
}