using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using eCommerce.OrdersService.BLL.DTOs;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Polly.Bulkhead;
using Polly.CircuitBreaker;
using Polly.Timeout;

namespace eCommerce.OrdersService.BLL.HttpClients;

public class UsersMicroserviceClient(
    HttpClient httpClient,
    ILogger<UsersMicroserviceClient> logger,
    IDistributedCache distributedCache)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<UsersMicroserviceClient> _logger = logger;
    private readonly IDistributedCache _distributedCache = distributedCache;

    public async Task<AppResponse<AppUserDto>> GetUserByIdAsync(Guid userId)
    {
        try
        {
            var cacheKey = $"userId: {userId}";
            var cachedUser = await _distributedCache.GetStringAsync(cacheKey);

            if (cachedUser is not null)
            {
                var userFromCache = JsonSerializer.Deserialize<AppUserDto>(cachedUser);
                
                return AppResponse<AppUserDto>.Success(userFromCache);
            }

            var response = await _httpClient.GetAsync($"gateway/users/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                var errors = await response.Content.ReadAsStringAsync();

                _logger.LogWarning($"Users API returned {response.StatusCode}: {errors}");

                return AppResponse<AppUserDto>.Failure(null,
                    $"Users API failed with status {response.StatusCode}",
                    new[] { "Remote service error" });
            }

            var result = await response.Content.ReadFromJsonAsync<AppResponse<AppUserDto>>(new JsonSerializerOptions()
                { PropertyNameCaseInsensitive = true });

            if (result is null)
            {
                return AppResponse<AppUserDto>.Failure(null, "Users API returns empty response");
            }

            return result;
        }
        catch (BulkheadRejectedException)
        {
            _logger.LogError($"Users API is overloaded (Bulkhead Full). Request for user {userId} rejected.");

            return AppResponse<AppUserDto>.Failure(null,
                "The system is currently under heavy load. Please try again later.");
        }
        catch (BrokenCircuitException)
        {
            _logger.LogError($"Users API circuit is OPEN. Request for user {userId} blocked.");

            return AppResponse<AppUserDto>.Failure(null,
                "Users Service is temporarily unavailable (Circuit Open). Please try again later.");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, $"Network error calling Users API for user {userId}");

            return AppResponse<AppUserDto>.Failure(null,
                "Unable to reach Users Service. Please check network connection.");
        }
        catch (TimeoutRejectedException ex)
        {
            _logger.LogError(ex, $"Timeout occurred on Users API for user {userId}");

            return AppResponse<AppUserDto>.Failure(null,
                "Timeout occurred. Users API under heavy load");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Unexpected error calling Users API for user {userId}");

            return AppResponse<AppUserDto>.Failure(null, "An unexpected error occurred while verifying user.");
        }
    }
}