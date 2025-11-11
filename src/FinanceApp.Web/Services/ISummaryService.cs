using FinanceApp.Shared.DTOs;

namespace FinanceApp.Web.Services;

public interface ISummaryService
{
    Task<MonthSummaryDto?> GetMonthSummaryAsync(Guid organizationId);
}
