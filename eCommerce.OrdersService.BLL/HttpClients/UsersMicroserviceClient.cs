using System.Net;
using System.Net.Http.Json;
using eCommerce.OrdersService.BLL.DTOs;

namespace eCommerce.OrdersService.BLL.HttpClients;

public class UsersMicroserviceClient(HttpClient httpClient)
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<AppResponse<AppUserDto>> GetUserByIdAsync(Guid userId)
    {
        var response = await _httpClient.GetAsync($"api/users/{userId}");
        var result = await response.Content.ReadFromJsonAsync<AppResponse<AppUserDto>>();

        if (!response.IsSuccessStatusCode || result is null)
        {
            //TODO better failed result
            return AppResponse<AppUserDto>.Failure(null, result.Message, result.Errors);
        }
        
        return result;
    }
}