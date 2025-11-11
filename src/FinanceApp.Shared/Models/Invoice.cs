namespace FinanceApp.Shared.Models;

public class Invoice
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public string Vendor { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "EUR";
    public DateTime DueAt { get; set; }
    public InvoiceStatus Status { get; set; }
    public string? Category { get; set; }
    public string? Notes { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<Attachment> Attachments { get; set; } = new();
}

public enum InvoiceStatus
{
    Open,
    Paid
}
