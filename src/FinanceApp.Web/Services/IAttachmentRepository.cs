using FinanceApp.Shared.Models;

namespace FinanceApp.Web.Services;

public interface IAttachmentRepository
{
    Task<List<Attachment>> GetByInvoiceIdAsync(Guid invoiceId);
    Task<Attachment?> CreateAsync(Guid invoiceId, string objectPath, string? contentType, long? sizeBytes);
    Task<bool> DeleteAsync(Guid id);
}
