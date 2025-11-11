namespace FinanceApp.Shared.DTOs;

using FinanceApp.Shared.Models;

public class CreateInvoiceDto
{
    public string Vendor { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "EUR";
    public DateTime DueAt { get; set; }
    public InvoiceStatus Status { get; set; }
    public string? Category { get; set; }
    public string? Notes { get; set; }
}

public class UpdateInvoiceDto : CreateInvoiceDto
{
    public Guid Id { get; set; }
}
