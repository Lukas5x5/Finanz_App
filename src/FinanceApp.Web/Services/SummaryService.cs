using FinanceApp.Shared.DTOs;
using System.Text.Json;

namespace FinanceApp.Web.Services;

public class SummaryService : ISummaryService
{
    private readonly Supabase.Client _client;

    public SummaryService(Supabase.Client client)
    {
        _client = client;
    }

    public async Task<MonthSummaryDto?> GetMonthSummaryAsync(Guid organizationId)
    {
        try
        {
            var response = await _client.Rpc("rpc_get_month_summary", new Dictionary<string, object>
            {
                { "org_id", organizationId }
            });

            if (response == null || response.Content == null)
            {
                Console.WriteLine("RPC returned null or no content");
                return new MonthSummaryDto();
            }

            var json = JsonSerializer.Deserialize<JsonElement>(response.Content);

            var summary = new MonthSummaryDto
            {
                TotalMonthly = json.GetProperty("total_monthly").GetDecimal(),
                OpenInvoices = json.GetProperty("open_invoices").GetDecimal(),
                Next30DaysCashOut = json.GetProperty("next_30_days_cash_out").GetDecimal()
            };

            // Parse upcoming bindings
            if (json.TryGetProperty("upcoming_bindings", out var bindingsElement) &&
                bindingsElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var binding in bindingsElement.EnumerateArray())
                {
                    summary.UpcomingBindings.Add(new UpcomingBindingDto
                    {
                        CostItemId = Guid.Parse(binding.GetProperty("cost_item_id").GetString()!),
                        Name = binding.GetProperty("name").GetString()!,
                        BindingEndsAt = binding.GetProperty("binding_ends_at").GetDateTime(),
                        DaysUntilEnd = binding.GetProperty("days_until_end").GetInt32()
                    });
                }
            }

            return summary;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error getting month summary: {ex.Message}");
            // Return empty summary instead of null to avoid UI errors
            return new MonthSummaryDto
            {
                TotalMonthly = 0,
                OpenInvoices = 0,
                Next30DaysCashOut = 0
            };
        }
    }
}
