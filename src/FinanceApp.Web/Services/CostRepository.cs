using FinanceApp.Shared.Models;
using FinanceApp.Shared.DTOs;
using Postgrest.Attributes;
using Postgrest.Models;
using static Postgrest.Constants;

namespace FinanceApp.Web.Services;

public class CostRepository : ICostRepository
{
    private readonly Supabase.Client _client;

    public CostRepository(Supabase.Client client)
    {
        _client = client;
    }

    public async Task<List<CostItem>> GetAllAsync(Guid organizationId)
    {
        try
        {
            var response = await _client
                .From<CostItemModel>()
                .Filter("organization_id", Operator.Equals, organizationId.ToString())
                .Order("name", Ordering.Ascending)
                .Get();

            return response.Models.Select(MapToDomain).ToList();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error getting cost items: {ex.Message}");
            return new List<CostItem>();
        }
    }

    public async Task<CostItem?> GetByIdAsync(Guid id)
    {
        try
        {
            var response = await _client
                .From<CostItemModel>()
                .Filter("id", Operator.Equals, id.ToString())
                .Single();

            return response != null ? MapToDomain(response) : null;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error getting cost item: {ex.Message}");
            return null;
        }
    }

    public async Task<CostItem?> CreateAsync(Guid organizationId, CreateCostItemDto dto)
    {
        try
        {
            var userId = _client.Auth.CurrentUser?.Id;
            if (userId == null) return null;

            var model = new CostItemModel
            {
                OrganizationId = organizationId.ToString(),
                Name = dto.Name,
                Category = dto.Category,
                Amount = dto.Amount,
                Currency = dto.Currency,
                Cycle = dto.Cycle.ToString(),
                HasBinding = dto.HasBinding,
                BindingEndsAt = dto.BindingEndsAt,
                PaymentMethod = dto.PaymentMethod,
                Notes = dto.Notes,
                Tags = dto.Tags.ToArray(),
                CreatedBy = userId
            };

            var response = await _client
                .From<CostItemModel>()
                .Insert(model);

            var created = response.Models.FirstOrDefault();
            return created != null ? MapToDomain(created) : null;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error creating cost item: {ex.Message}");
            return null;
        }
    }

    public async Task<CostItem?> UpdateAsync(UpdateCostItemDto dto)
    {
        try
        {
            var model = new CostItemModel
            {
                Name = dto.Name,
                Category = dto.Category,
                Amount = dto.Amount,
                Currency = dto.Currency,
                Cycle = dto.Cycle.ToString(),
                HasBinding = dto.HasBinding,
                BindingEndsAt = dto.BindingEndsAt,
                PaymentMethod = dto.PaymentMethod,
                Notes = dto.Notes,
                Tags = dto.Tags.ToArray()
            };

            var response = await _client
                .From<CostItemModel>()
                .Filter("id", Operator.Equals, dto.Id.ToString())
                .Update(model);

            var updated = response.Models.FirstOrDefault();
            return updated != null ? MapToDomain(updated) : null;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error updating cost item: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        try
        {
            await _client
                .From<CostItemModel>()
                .Filter("id", Operator.Equals, id.ToString())
                .Delete();

            return true;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error deleting cost item: {ex.Message}");
            return false;
        }
    }

    public async Task<string> ExportToCsvAsync(Guid organizationId)
    {
        try
        {
            var response = await _client.Rpc("rpc_export_costs_csv", new Dictionary<string, object>
            {
                { "org_id", organizationId }
            });

            return response?.Content ?? string.Empty;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error exporting costs: {ex.Message}");
            return string.Empty;
        }
    }

    private static CostItem MapToDomain(CostItemModel model)
    {
        return new CostItem
        {
            Id = Guid.Parse(model.Id),
            OrganizationId = Guid.Parse(model.OrganizationId),
            Name = model.Name,
            Category = model.Category,
            Amount = model.Amount,
            Currency = model.Currency,
            Cycle = Enum.Parse<BillingCycle>(model.Cycle),
            HasBinding = model.HasBinding,
            BindingEndsAt = model.BindingEndsAt,
            PaymentMethod = model.PaymentMethod,
            Notes = model.Notes,
            Tags = model.Tags?.ToList() ?? new List<string>(),
            CreatedBy = Guid.Parse(model.CreatedBy),
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt
        };
    }
}

[Table("cost_items")]
public class CostItemModel : BaseModel
{
    [PrimaryKey("id")]
    public string Id { get; set; } = string.Empty;

    [Column("organization_id")]
    public string OrganizationId { get; set; } = string.Empty;

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("category")]
    public string? Category { get; set; }

    [Column("amount")]
    public decimal Amount { get; set; }

    [Column("currency")]
    public string Currency { get; set; } = "EUR";

    [Column("cycle")]
    public string Cycle { get; set; } = string.Empty;

    [Column("has_binding")]
    public bool HasBinding { get; set; }

    [Column("binding_ends_at")]
    public DateTime? BindingEndsAt { get; set; }

    [Column("payment_method")]
    public string? PaymentMethod { get; set; }

    [Column("notes")]
    public string? Notes { get; set; }

    [Column("tags")]
    public string[]? Tags { get; set; }

    [Column("created_by")]
    public string CreatedBy { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}
