using FinanceApp.Shared.Models;
using FinanceApp.Shared.DTOs;

namespace FinanceApp.Web.Services;

public interface ICostRepository
{
    Task<List<CostItem>> GetAllAsync(Guid organizationId);
    Task<CostItem?> GetByIdAsync(Guid id);
    Task<CostItem?> CreateAsync(Guid organizationId, CreateCostItemDto dto);
    Task<CostItem?> UpdateAsync(UpdateCostItemDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task<string> ExportToCsvAsync(Guid organizationId);
}
