using FinanceApp.Shared.Models;
using FinanceApp.Shared.DTOs;

namespace FinanceApp.Web.Services;

public interface IOrganizationService
{
    Task<List<Organization>> GetUserOrganizationsAsync();
    Task<Organization?> GetOrganizationAsync(Guid id);
    Task<Organization?> CreateOrganizationAsync(CreateOrganizationDto dto);
    Task<Organization?> CreatePersonalOrganizationAsync();
    Task<bool> DeleteOrganizationAsync(Guid id);
}
