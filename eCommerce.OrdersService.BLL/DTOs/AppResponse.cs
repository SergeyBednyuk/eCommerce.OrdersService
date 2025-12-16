namespace eCommerce.OrdersService.BLL.DTOs;

public class AppResponse<T> where T : class
{
    public bool IsSuccess { get; set; }

    public T? Date { get; set; }

    public string? Message { get; set; }

    public IEnumerable<string>? Errors { get; set; }
    
    private AppResponse(bool isSuccess, T? date, string? message, IEnumerable<string>? errors)
    {
        IsSuccess = isSuccess;
        Date = date;
        Message = message;
        Errors = errors;
    }

    public static AppResponse<T> Success(T? data, string message = null)
    {
        return new AppResponse<T>(true, data, message, null);
    }

    public static AppResponse<T> Failure(T? data, string message, IEnumerable<string>? errors = null)
    {
        return new AppResponse<T>(false, data, message, errors);
    }
}