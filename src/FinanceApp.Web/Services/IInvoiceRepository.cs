using FinanceApp.Shared.Models;
using FinanceApp.Shared.DTOs;

namespace FinanceApp.Web.Services;

public interface IInvoiceRepository
{
    Task<List<Invoice>> GetAllAsync(Guid organizationId);
    Task<Invoice?> GetByIdAsync(Guid id);
    Task<Invoice?> CreateAsync(Guid organizationId, CreateInvoiceDto dto);
    Task<Invoice?> UpdateAsync(UpdateInvoiceDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task<string> ExportToCsvAsync(Guid organizationId);
}
