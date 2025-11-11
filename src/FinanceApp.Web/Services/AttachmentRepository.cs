using FinanceApp.Shared.Models;
using Postgrest.Attributes;
using Postgrest.Models;
using static Postgrest.Constants;

namespace FinanceApp.Web.Services;

public class AttachmentRepository : IAttachmentRepository
{
    private readonly Supabase.Client _client;

    public AttachmentRepository(Supabase.Client client)
    {
        _client = client;
    }

    public async Task<List<Attachment>> GetByInvoiceIdAsync(Guid invoiceId)
    {
        try
        {
            var response = await _client
                .From<AttachmentModel>()
                .Filter("invoice_id", Operator.Equals, invoiceId.ToString())
                .Order("created_at", Ordering.Descending)
                .Get();

            return response.Models.Select(MapToDomain).ToList();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error getting attachments: {ex.Message}");
            return new List<Attachment>();
        }
    }

    public async Task<Attachment?> CreateAsync(Guid invoiceId, string objectPath, string? contentType, long? sizeBytes)
    {
        try
        {
            var userId = _client.Auth.CurrentUser?.Id;
            if (userId == null) return null;

            var model = new AttachmentModel
            {
                InvoiceId = invoiceId.ToString(),
                ObjectPath = objectPath,
                ContentType = contentType,
                SizeBytes = sizeBytes,
                CreatedBy = userId
            };

            var response = await _client
                .From<AttachmentModel>()
                .Insert(model);

            var created = response.Models.FirstOrDefault();
            return created != null ? MapToDomain(created) : null;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error creating attachment: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        try
        {
            await _client
                .From<AttachmentModel>()
                .Filter("id", Operator.Equals, id.ToString())
                .Delete();

            return true;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error deleting attachment: {ex.Message}");
            return false;
        }
    }

    private static Attachment MapToDomain(AttachmentModel model)
    {
        return new Attachment
        {
            Id = Guid.Parse(model.Id),
            InvoiceId = Guid.Parse(model.InvoiceId),
            ObjectPath = model.ObjectPath,
            ContentType = model.ContentType,
            SizeBytes = model.SizeBytes,
            CreatedBy = Guid.Parse(model.CreatedBy),
            CreatedAt = model.CreatedAt
        };
    }
}

[Table("attachments")]
public class AttachmentModel : BaseModel
{
    [PrimaryKey("id")]
    public string Id { get; set; } = string.Empty;

    [Column("invoice_id")]
    public string InvoiceId { get; set; } = string.Empty;

    [Column("object_path")]
    public string ObjectPath { get; set; } = string.Empty;

    [Column("content_type")]
    public string? ContentType { get; set; }

    [Column("size_bytes")]
    public long? SizeBytes { get; set; }

    [Column("created_by")]
    public string CreatedBy { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}
