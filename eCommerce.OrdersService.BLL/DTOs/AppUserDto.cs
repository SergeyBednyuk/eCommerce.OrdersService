namespace eCommerce.OrdersService.BLL.DTOs;

public record AppUserDto(Guid Id, string? Email, string? FirstName, string? LastName, string? Gender)
{
    public AppUserDto() : this(Guid.Empty, null, null, null, null)
    {
    }
}