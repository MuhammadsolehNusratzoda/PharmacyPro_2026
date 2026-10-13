namespace PharmacyPro.Application.Responses;

public class PagedResponse<T> : Response<List<T>>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);

    public static PagedResponse<T> Create(List<T> data, int totalCount, int pageNumber, int pageSize) => new()
    {
        IsSuccess = true,
        StatusCode = 200,
        Data = data,
        TotalCount = totalCount,
        PageNumber = pageNumber,
        PageSize = pageSize
    };
}