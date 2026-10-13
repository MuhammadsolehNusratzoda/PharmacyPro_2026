using PharmacyPro.Application.DTOs;
using PharmacyPro.Application.Filters;
using PharmacyPro.Application.Responses;

namespace PharmacyPro.Application.Interfaces;

public interface IFirmService
{
    Task<PagedResponse<FirmDto>> GetAllAsync(FirmFilter filter);
    Task<Response<FirmDto>> GetByIdAsync(int id);
    Task<Response<FirmDto>> CreateAsync(CreateFirmDto dto);
    Task<Response<FirmDto>> UpdateAsync(int id, UpdateFirmDto dto);
    Task<Response<bool>> DeleteAsync(int id);
}

public interface ISaleService
{
    Task<PagedResponse<SaleDto>> GetAllAsync(SaleFilter filter);
    Task<Response<SaleDto>> GetByIdAsync(int id);
    Task<Response<SaleDto>> CreateAsync(CreateSaleDto dto);
    Task<Response<SaleDto>> UpdateAsync(int id, UpdateSaleDto dto);
    Task<Response<bool>> DeleteAsync(int id);
}

public interface IExpenseCategoryService
{
    Task<Response<List<ExpenseCategoryDto>>> GetAllAsync();
    Task<Response<ExpenseCategoryDto>> CreateAsync(CreateExpenseCategoryDto dto);
    Task<Response<ExpenseCategoryDto>> UpdateAsync(int id, CreateExpenseCategoryDto dto);
    Task<Response<bool>> DeleteAsync(int id);
}

public interface IExpenseService
{
    Task<PagedResponse<ExpenseDto>> GetAllAsync(ExpenseFilter filter);
    Task<Response<ExpenseDto>> GetByIdAsync(int id);
    Task<Response<ExpenseDto>> CreateAsync(CreateExpenseDto dto);
    Task<Response<ExpenseDto>> UpdateAsync(int id, UpdateExpenseDto dto);
    Task<Response<bool>> DeleteAsync(int id);
}

public interface IFirmPaymentService
{
    Task<PagedResponse<FirmPaymentDto>> GetAllAsync(FirmPaymentFilter filter);
    Task<Response<FirmPaymentDto>> GetByIdAsync(int id);
    Task<Response<FirmPaymentDto>> CreateAsync(CreateFirmPaymentDto dto);
    Task<Response<FirmPaymentDto>> UpdateAsync(int id, UpdateFirmPaymentDto dto);
    Task<Response<bool>> DeleteAsync(int id);
}

public interface IReportService
{
    Task<Response<MonthlyReportDto>> GetMonthlyReportAsync(int year, int month);
    Task<Response<List<WeeklySaleDto>>> GetWeeklySalesAsync(int year, int month);
}