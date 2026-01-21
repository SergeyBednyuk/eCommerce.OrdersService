using System.Collections.Concurrent;
using System.Net.Http.Json;
using System.Text.Json;
using eCommerce.OrdersService.BLL.DTOs;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Polly.Bulkhead;
using Polly.CircuitBreaker;
using Polly.Timeout;

namespace eCommerce.OrdersService.BLL.HttpClients;

public class ProductsMicroserviceClient(
    HttpClient httpClient,
    ILogger<ProductsMicroserviceClient> logger,
    IDistributedCache distributedCache)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<ProductsMicroserviceClient> _logger = logger;
    private readonly IDistributedCache _distributedCache = distributedCache;

    private static string GetProductCacheKey(Guid productId) => $"product:{productId}";

    public async Task<AppResponse<IEnumerable<ProductDTO>>> GetProductsByIdsAsync(IEnumerable<Guid> ids)
    {
        var requestedIds = ids.Distinct().ToList();
        var foundProducts = new ConcurrentBag<ProductDTO>();
        var missingIds = new List<Guid>();

        try
        {
            // 1. getting missing product ids
            // IDistributedCache doesn't have MultiGet, so we ran all in parallel
            var cacheTasks = requestedIds.Select(async id =>
            {
                var cacheKey = GetProductCacheKey(id);
                var cachedProduct = await _distributedCache.GetStringAsync(cacheKey);

                if (!string.IsNullOrEmpty(cachedProduct))
                {
                    var product = JsonSerializer.Deserialize<ProductDTO>(cachedProduct);
                    if (product is not null)
                    {
                        foundProducts.Add(product);
                        return Guid.Empty;
                    }
                }

                return id;
            });

            var results = await Task.WhenAll(cacheTasks);
            missingIds = results.Where(x => x != Guid.Empty).ToList();

            // 2. Fast path. All products in cache
            if (!missingIds.Any())
            {
                return AppResponse<IEnumerable<ProductDTO>>.Success(foundProducts);
            }

            string url = $"api/products/search/batch";
            var payload = new
            {
                Ids = missingIds,
            };

            // 3. There are missing product. 

            var response = await _httpClient.PutAsJsonAsync(url, payload);

            var apiResult =
                await response.Content.ReadFromJsonAsync<AppResponse<IEnumerable<ProductDTO>>>(
                    new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            if (apiResult is null || !apiResult.IsSuccess || apiResult.Data is null || !apiResult.Data.Any())
            {
                _logger.LogWarning(
                    $"Failed to get products for {string.Join(", ", missingIds)} ids from Product Service");
                return AppResponse<IEnumerable<ProductDTO>>.Failure(null, apiResult?.Message ?? "Product API error",
                    apiResult?.Errors);
            }

            // 4. Update cache with missing data
            var missingProducts = apiResult.Data.ToList();
            var setCacheTasks = missingProducts.Select(product =>
            {
                var productJson = JsonSerializer.Serialize(product);
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                };

                return _distributedCache.SetStringAsync(GetProductCacheKey(product.Id), productJson, cacheOptions);
            });
            await Task.WhenAll(setCacheTasks);

            // 5. merge all 
            var allProducts = foundProducts.Concat(missingProducts);

            return AppResponse<IEnumerable<ProductDTO>>.Success(allProducts);
        }
        catch (BulkheadRejectedException)
        {
            _logger.LogError(
                $"Product API is overloaded (Bulkhead Full). Getting request for {string.Join(", ", requestedIds)} rejected.");

            return AppResponse<IEnumerable<ProductDTO>>.Failure(null,
                "The system is currently under heavy load. Please try again later.");
        }
        catch (BrokenCircuitException)
        {
            _logger.LogError($"Product API circuit is OPEN. Request blocked.");

            return AppResponse<IEnumerable<ProductDTO>>.Failure(null,
                "Product Service is temporarily unavailable (Circuit Open). Please try again later.");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, $"Network error calling Product API");

            return AppResponse<IEnumerable<ProductDTO>>.Failure(null,
                "Unable to reach Product Service. Please check network connection.");
        }
        catch (TimeoutRejectedException ex)
        {
            _logger.LogError(ex, $"Timeout occurred on Product API");

            return AppResponse<IEnumerable<ProductDTO>>.Failure(null,
                "Timeout occurred. Product API under heavy load");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                $"Network error communicating with Product Service for {string.Join(", ", requestedIds)}");
            return AppResponse<IEnumerable<ProductDTO>>.Failure(null,
                $"Network error communicating with Product Service for {string.Join(", ", requestedIds)}");
        }
    }

    public async Task<AppResponse<IEnumerable<ProductDTO>>> UpdateProductStockByIdAsync(UpdateStockRequest request)
    {
        try
        {
            string url = $"api/products/stock/batch-update";

            var response = await _httpClient.PostAsJsonAsync(url, request);

            var result = await response.Content.ReadFromJsonAsync<AppResponse<IEnumerable<ProductDTO>>>(
                new JsonSerializerOptions()
                    { PropertyNameCaseInsensitive = true });

            if (result is not null && result.IsSuccess && result.Data is not null)
            {
                var removeCacheTasks =
                    result.Data.Select(product => _distributedCache.RemoveAsync(GetProductCacheKey(product.Id)));

                await Task.WhenAll(removeCacheTasks);
                
                return result;
            }

            _logger.LogWarning("Failed to adjust stock thought Orders API. Errors: {errors} message {message} ",
                result.Errors, result.Message);

            return AppResponse<IEnumerable<ProductDTO>>.Failure(null, result?.Message ?? "Product API error", result?.Errors);
        }
        catch (BulkheadRejectedException)
        {
            _logger.LogError(
                $"Product API is overloaded (Bulkhead Full). {nameof(request.Reduce)} request is rejected.");

            return AppResponse<IEnumerable<ProductDTO>>.Failure(null,
                "The system is currently under heavy load. Please try again later.");
        }
        catch (BrokenCircuitException)
        {
            _logger.LogError($"Product API circuit is OPEN. Request blocked.");

            return AppResponse<IEnumerable<ProductDTO>>.Failure(null,
                "Product Service is temporarily unavailable (Circuit Open). Please try again later.");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, $"Network error calling Product API");

            return AppResponse<IEnumerable<ProductDTO>>.Failure(null,
                "Unable to reach Product Service. Please check network connection.");
        }
        catch (TimeoutRejectedException ex)
        {
            _logger.LogError(ex, $"Timeout occurred on Product API");

            return AppResponse<IEnumerable<ProductDTO>>.Failure(null,
                "Timeout occurred. Product API under heavy load");
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