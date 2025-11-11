namespace FinanceApp.Shared.DTOs;

public class MonthSummaryDto
{
    public decimal TotalMonthly { get; set; }
    public decimal OpenInvoices { get; set; }
    public decimal Next30DaysCashOut { get; set; }
    public List<UpcomingBindingDto> UpcomingBindings { get; set; } = new();
}

public class UpcomingBindingDto
{
    public Guid CostItemId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime BindingEndsAt { get; set; }
    public int DaysUntilEnd { get; set; }
}
