using FinanceApp.Shared.Models;
using FinanceApp.Shared.DTOs;
using Postgrest.Attributes;
using Postgrest.Models;

namespace FinanceApp.Web.Services;

public class OrganizationService : IOrganizationService
{
    private readonly Supabase.Client _client;

    public OrganizationService(Supabase.Client client)
    {
        _client = client;
    }

    public async Task<List<Organization>> GetUserOrganizationsAsync()
    {
        try
        {
            var userId = _client.Auth.CurrentUser?.Id;
            if (userId == null)
            {
                Console.WriteLine("No current user");
                return new List<Organization>();
            }

            // Get organizations where user is owner
            var response = await _client
                .From<OrganizationModel>()
                .Filter("owner_id", Postgrest.Constants.Operator.Equals, userId)
                .Get();

            if (response?.Models == null || !response.Models.Any())
            {
                Console.WriteLine("No organizations found");
                return new List<Organization>();
            }

            return response.Models.Select(m => new Organization
            {
                Id = Guid.Parse(m.Id),
                Name = m.Name,
                Type = Enum.Parse<OrganizationType>(m.Type),
                OwnerId = Guid.Parse(m.OwnerId),
                CreatedAt = m.CreatedAt
            }).ToList();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error getting organizations: {ex.Message}");
            return new List<Organization>();
        }
    }

    public async Task<Organization?> GetOrganizationAsync(Guid id)
    {
        try
        {
            var response = await _client
                .From<OrganizationModel>()
                .Filter("id", Postgrest.Constants.Operator.Equals, id.ToString())
                .Single();

            if (response == null) return null;

            return new Organization
            {
                Id = Guid.Parse(response.Id),
                Name = response.Name,
                Type = Enum.Parse<OrganizationType>(response.Type),
                OwnerId = Guid.Parse(response.OwnerId),
                CreatedAt = response.CreatedAt
            };
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error getting organization: {ex.Message}");
            return null;
        }
    }

    public async Task<Organization?> CreateOrganizationAsync(CreateOrganizationDto dto)
    {
        try
        {
            var userId = _client.Auth.CurrentUser?.Id;
            if (userId == null) return null;

            var model = new OrganizationModel
            {
                Name = dto.Name,
                Type = dto.Type.ToString(),
                OwnerId = userId
            };

            var response = await _client
                .From<OrganizationModel>()
                .Insert(model);

            var created = response.Models.FirstOrDefault();
            if (created == null) return null;

            return new Organization
            {
                Id = Guid.Parse(created.Id),
                Name = created.Name,
                Type = Enum.Parse<OrganizationType>(created.Type),
                OwnerId = Guid.Parse(created.OwnerId),
                CreatedAt = created.CreatedAt
            };
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error creating organization: {ex.Message}");
            return null;
        }
    }

    public async Task<Organization?> CreatePersonalOrganizationAsync()
    {
        try
        {
            var response = await _client.Rpc("rpc_create_personal_organization", new Dictionary<string, object>());
            if (response != null && response.Content != null)
            {
                var orgId = Guid.Parse(response.Content.Trim('"'));
                return await GetOrganizationAsync(orgId);
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error creating personal organization: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> DeleteOrganizationAsync(Guid id)
    {
        try
        {
            await _client
                .From<OrganizationModel>()
                .Filter("id", Postgrest.Constants.Operator.Equals, id.ToString())
                .Delete();

            return true;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error deleting organization: {ex.Message}");
            return false;
        }
    }
}

// Postgrest Models
[Table("organizations")]
public class OrganizationModel : BaseModel
{
    [PrimaryKey("id")]
    public string Id { get; set; } = string.Empty;

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("type")]
    public string Type { get; set; } = string.Empty;

    [Column("owner_id")]
    public string OwnerId { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}
