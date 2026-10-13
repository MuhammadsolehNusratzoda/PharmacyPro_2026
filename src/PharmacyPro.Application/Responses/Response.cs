namespace PharmacyPro.Application.Responses;

public class Response<T>
{
    public bool IsSuccess { get; set; }
    public int StatusCode { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }

    public static Response<T> Success(T data, string? message = null, int statusCode = 200) => new()
    {
        IsSuccess = true,
        StatusCode = statusCode,
        Message = message,
        Data = data
    };

    public static Response<T> Fail(string message, int statusCode = 400, List<string>? errors = null) => new()
    {
        IsSuccess = false,
        StatusCode = statusCode,
        Message = message,
        Errors = errors
    };
}