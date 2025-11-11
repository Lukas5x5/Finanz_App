namespace FinanceApp.Shared.DTOs;

using FinanceApp.Shared.Models;

public class CreateOrganizationDto
{
    public string Name { get; set; } = string.Empty;
    public OrganizationType Type { get; set; }
}
