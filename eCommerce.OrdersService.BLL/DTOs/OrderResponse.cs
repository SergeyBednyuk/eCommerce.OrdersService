namespace eCommerce.OrdersService.BLL.DTOs;

public class OrderResponse<T> where T : class
{
    public bool IsSuccess { get; set; }
    public T?Data { get; set; }
    public string? Message { get; set; }
    public IEnumerable<string>? Errors { get; set; }

    private OrderResponse(bool isSuccess, T? data, string? message, IEnumerable<string>? errors)
    {
        IsSuccess = isSuccess;
        Data = data;
        Message = message;
        Errors = errors;
    }

    public static OrderResponse<T> Success(T data, string? message = null)
    {
        return new OrderResponse<T>(true, data, message,  null);
    }

    public static OrderResponse<T> Failure(T? data = null, string? message = null, IEnumerable<string>? errors = null)
    {
        return new OrderResponse<T>(false, data, message, errors);
    }
}