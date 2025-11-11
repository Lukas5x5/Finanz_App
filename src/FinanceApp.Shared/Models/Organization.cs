namespace FinanceApp.Shared.Models;

public class Organization
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public OrganizationType Type { get; set; }
    public Guid OwnerId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public enum OrganizationType
{
    Personal,
    Business
}
