namespace FinanceApp.Shared.Models;

public class CostItem
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Category { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "EUR";
    public BillingCycle Cycle { get; set; }
    public bool HasBinding { get; set; }
    public DateTime? BindingEndsAt { get; set; }
    public string? PaymentMethod { get; set; }
    public string? Notes { get; set; }
    public List<string> Tags { get; set; } = new();
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public enum BillingCycle
{
    Monthly,
    Yearly
}
