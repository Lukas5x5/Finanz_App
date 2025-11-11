using FinanceApp.Shared.Models;
using FinanceApp.Shared.DTOs;
using Postgrest.Attributes;
using Postgrest.Models;
using static Postgrest.Constants;

namespace FinanceApp.Web.Services;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly Supabase.Client _client;

    public InvoiceRepository(Supabase.Client client)
    {
        _client = client;
    }

    public async Task<List<Invoice>> GetAllAsync(Guid organizationId)
    {
        try
        {
            var response = await _client
                .From<InvoiceModel>()
                .Filter("organization_id", Operator.Equals, organizationId.ToString())
                .Order("due_at", Ordering.Descending)
                .Get();

            return response.Models.Select(MapToDomain).ToList();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error getting invoices: {ex.Message}");
            return new List<Invoice>();
        }
    }

    public async Task<Invoice?> GetByIdAsync(Guid id)
    {
        try
        {
            var response = await _client
                .From<InvoiceModel>()
                .Filter("id", Operator.Equals, id.ToString())
                .Single();

            return response != null ? MapToDomain(response) : null;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error getting invoice: {ex.Message}");
            return null;
        }
    }

    public async Task<Invoice?> CreateAsync(Guid organizationId, CreateInvoiceDto dto)
    {
        try
        {
            var userId = _client.Auth.CurrentUser?.Id;
            if (userId == null) return null;

            var model = new InvoiceModel
            {
                OrganizationId = organizationId.ToString(),
                Vendor = dto.Vendor,
                Amount = dto.Amount,
                Currency = dto.Currency,
                DueAt = dto.DueAt,
                Status = dto.Status.ToString(),
                Category = dto.Category,
                Notes = dto.Notes,
                CreatedBy = userId
            };

            var response = await _client
                .From<InvoiceModel>()
                .Insert(model);

            var created = response.Models.FirstOrDefault();
            return created != null ? MapToDomain(created) : null;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error creating invoice: {ex.Message}");
            return null;
        }
    }

    public async Task<Invoice?> UpdateAsync(UpdateInvoiceDto dto)
    {
        try
        {
            var model = new InvoiceModel
            {
                Vendor = dto.Vendor,
                Amount = dto.Amount,
                Currency = dto.Currency,
                DueAt = dto.DueAt,
                Status = dto.Status.ToString(),
                Category = string.IsNullOrWhiteSpace(dto.Category) ? null : dto.Category,
                Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes
            };

            var response = await _client
                .From<InvoiceModel>()
                .Filter("id", Operator.Equals, dto.Id.ToString())
                .Update(model);

            var updated = response.Models.FirstOrDefault();
            return updated != null ? MapToDomain(updated) : null;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error updating invoice: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        try
        {
            await _client
                .From<InvoiceModel>()
                .Filter("id", Operator.Equals, id.ToString())
                .Delete();

            return true;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error deleting invoice: {ex.Message}");
            return false;
        }
    }

    public async Task<string> ExportToCsvAsync(Guid organizationId)
    {
        try
        {
            var response = await _client.Rpc("rpc_export_invoices_csv", new Dictionary<string, object>
            {
                { "org_id", organizationId }
            });

            return response?.Content ?? string.Empty;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error exporting invoices: {ex.Message}");
            return string.Empty;
        }
    }

    private static Invoice MapToDomain(InvoiceModel model)
    {
        return new Invoice
        {
            Id = Guid.Parse(model.Id),
            OrganizationId = Guid.Parse(model.OrganizationId),
            Vendor = model.Vendor,
            Amount = model.Amount,
            Currency = model.Currency,
            DueAt = model.DueAt,
            Status = Enum.Parse<InvoiceStatus>(model.Status),
            Category = model.Category,
            Notes = model.Notes,
            CreatedBy = Guid.Parse(model.CreatedBy),
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt
        };
    }
}

[Table("invoices")]
public class InvoiceModel : BaseModel
{
    [PrimaryKey("id")]
    public string Id { get; set; } = string.Empty;

    [Column("organization_id")]
    public string OrganizationId { get; set; } = string.Empty;

    [Column("vendor")]
    public string Vendor { get; set; } = string.Empty;

    [Column("amount")]
    public decimal Amount { get; set; }

    [Column("currency")]
    public string Currency { get; set; } = "EUR";

    [Column("due_at")]
    public DateTime DueAt { get; set; }

    [Column("status")]
    public string Status { get; set; } = "Open";

    [Column("category")]
    public string? Category { get; set; }

    [Column("notes")]
    public string? Notes { get; set; }

    [Column("created_by")]
    public string CreatedBy { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}
