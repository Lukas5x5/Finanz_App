namespace FinanceApp.Shared.Models;

public class Attachment
{
    public Guid Id { get; set; }
    public Guid InvoiceId { get; set; }
    public string ObjectPath { get; set; } = string.Empty;
    public string? ContentType { get; set; }
    public long? SizeBytes { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}
