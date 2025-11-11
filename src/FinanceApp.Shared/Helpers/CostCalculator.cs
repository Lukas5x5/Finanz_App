namespace FinanceApp.Shared.Helpers;

using FinanceApp.Shared.Models;

/// <summary>
/// Helper class for calculating cost summaries
/// </summary>
public static class CostCalculator
{
    /// <summary>
    /// Calculates the monthly cost for a given cost item
    /// </summary>
    public static decimal CalculateMonthlyAmount(CostItem item)
    {
        return item.Cycle switch
        {
            BillingCycle.Monthly => item.Amount,
            BillingCycle.Yearly => item.Amount / 12m,
            _ => throw new ArgumentException($"Unknown billing cycle: {item.Cycle}")
        };
    }

    /// <summary>
    /// Calculates the total monthly cost for a list of cost items
    /// </summary>
    public static decimal CalculateTotalMonthly(IEnumerable<CostItem> items)
    {
        return items.Sum(CalculateMonthlyAmount);
    }

    /// <summary>
    /// Calculates the yearly cost for a given cost item
    /// </summary>
    public static decimal CalculateYearlyAmount(CostItem item)
    {
        return item.Cycle switch
        {
            BillingCycle.Monthly => item.Amount * 12m,
            BillingCycle.Yearly => item.Amount,
            _ => throw new ArgumentException($"Unknown billing cycle: {item.Cycle}")
        };
    }

    /// <summary>
    /// Calculates the total yearly cost for a list of cost items
    /// </summary>
    public static decimal CalculateTotalYearly(IEnumerable<CostItem> items)
    {
        return items.Sum(CalculateYearlyAmount);
    }

    /// <summary>
    /// Gets cost items with bindings ending within the specified number of days
    /// </summary>
    public static IEnumerable<CostItem> GetItemsWithUpcomingBindingEnd(
        IEnumerable<CostItem> items,
        int daysAhead = 30)
    {
        var today = DateTime.UtcNow.Date;
        var targetDate = today.AddDays(daysAhead);

        return items
            .Where(item => item.HasBinding
                && item.BindingEndsAt.HasValue
                && item.BindingEndsAt.Value >= today
                && item.BindingEndsAt.Value <= targetDate)
            .OrderBy(item => item.BindingEndsAt);
    }

    /// <summary>
    /// Gets invoices due within the specified number of days
    /// </summary>
    public static IEnumerable<Invoice> GetInvoicesDueWithinDays(
        IEnumerable<Invoice> invoices,
        int daysAhead = 30)
    {
        var today = DateTime.UtcNow.Date;
        var targetDate = today.AddDays(daysAhead);

        return invoices
            .Where(inv => inv.Status == InvoiceStatus.Open
                && inv.DueAt >= today
                && inv.DueAt <= targetDate)
            .OrderBy(inv => inv.DueAt);
    }

    /// <summary>
    /// Calculates total amount of open invoices
    /// </summary>
    public static decimal CalculateTotalOpenInvoices(IEnumerable<Invoice> invoices)
    {
        return invoices
            .Where(inv => inv.Status == InvoiceStatus.Open)
            .Sum(inv => inv.Amount);
    }

    /// <summary>
    /// Calculates total cash out for next N days (open invoices due)
    /// </summary>
    public static decimal CalculateNext30DaysCashOut(IEnumerable<Invoice> invoices)
    {
        return GetInvoicesDueWithinDays(invoices, 30).Sum(inv => inv.Amount);
    }
}
