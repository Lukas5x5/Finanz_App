namespace FinanceApp.Shared.DTOs;

using FinanceApp.Shared.Models;

public class CreateCostItemDto
{
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
}

public class UpdateCostItemDto : CreateCostItemDto
{
    public Guid Id { get; set; }
}
