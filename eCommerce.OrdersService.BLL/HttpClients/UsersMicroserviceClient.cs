using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using eCommerce.OrdersService.BLL.DTOs;
using Microsoft.Extensions.Logging;
using Polly.CircuitBreaker;

namespace eCommerce.OrdersService.BLL.HttpClients;

public class UsersMicroserviceClient(HttpClient httpClient, ILogger<UsersMicroserviceClient> logger)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<UsersMicroserviceClient> _logger = logger;

    public async Task<AppResponse<AppUserDto>> GetUserByIdAsync(Guid userId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/users/{userId}");

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
        catch (BrokenCircuitException)
        {
            _logger.LogError($"Users API circuit is OPEN. Request for user {userId} blocked.");

            return AppResponse<AppUserDto>.Failure(null,
                "Users Service is temporarily unavailable (Circuit Open). Please try again later.");
        }
        catch (HttpRequestException ex)
        {
            // GENERAL CATCH: Network is down, DNS failed, or connection refused (and retries failed)
            _logger.LogError(ex, $"Network error calling Users API for user {userId}");

            return AppResponse<AppUserDto>.Failure(null,
                "Unable to reach Users Service. Please check network connection.");
        }
        catch (Exception ex)
        {
            // FALLBACK: Serialization errors, timeouts, etc.
            _logger.LogError(ex, $"Unexpected error calling Users API for user {userId}");

            return AppResponse<AppUserDto>.Failure(null, "An unexpected error occurred while verifying user.");
        }
    }
}