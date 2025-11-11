using Xunit;
using FluentAssertions;
using FinanceApp.Shared.Helpers;
using FinanceApp.Shared.Models;

namespace FinanceApp.Tests;

public class CostCalculatorTests
{
    [Fact]
    public void CalculateMonthlyAmount_WithMonthlyCycle_ReturnsAmount()
    {
        // Arrange
        var costItem = new CostItem
        {
            Amount = 100m,
            Cycle = BillingCycle.Monthly
        };

        // Act
        var result = CostCalculator.CalculateMonthlyAmount(costItem);

        // Assert
        result.Should().Be(100m);
    }

    [Fact]
    public void CalculateMonthlyAmount_WithYearlyCycle_ReturnsDividedBy12()
    {
        // Arrange
        var costItem = new CostItem
        {
            Amount = 1200m,
            Cycle = BillingCycle.Yearly
        };

        // Act
        var result = CostCalculator.CalculateMonthlyAmount(costItem);

        // Assert
        result.Should().Be(100m);
    }

    [Fact]
    public void CalculateYearlyAmount_WithMonthlyCycle_ReturnsMultipliedBy12()
    {
        // Arrange
        var costItem = new CostItem
        {
            Amount = 100m,
            Cycle = BillingCycle.Monthly
        };

        // Act
        var result = CostCalculator.CalculateYearlyAmount(costItem);

        // Assert
        result.Should().Be(1200m);
    }

    [Fact]
    public void CalculateTotalMonthly_WithMixedCycles_ReturnsCorrectTotal()
    {
        // Arrange
        var items = new List<CostItem>
        {
            new() { Amount = 100m, Cycle = BillingCycle.Monthly },
            new() { Amount = 1200m, Cycle = BillingCycle.Yearly },
            new() { Amount = 50m, Cycle = BillingCycle.Monthly }
        };

        // Act
        var result = CostCalculator.CalculateTotalMonthly(items);

        // Assert
        result.Should().Be(250m); // 100 + 100 + 50
    }

    [Fact]
    public void GetItemsWithUpcomingBindingEnd_ReturnsOnlyItemsWithinTimeframe()
    {
        // Arrange
        var today = DateTime.UtcNow.Date;
        var items = new List<CostItem>
        {
            new()
            {
                Id = Guid.NewGuid(),
                HasBinding = true,
                BindingEndsAt = today.AddDays(10)
            },
            new()
            {
                Id = Guid.NewGuid(),
                HasBinding = true,
                BindingEndsAt = today.AddDays(50)
            },
            new()
            {
                Id = Guid.NewGuid(),
                HasBinding = false
            }
        };

        // Act
        var result = CostCalculator.GetItemsWithUpcomingBindingEnd(items, 30).ToList();

        // Assert
        result.Should().HaveCount(1);
        result[0].BindingEndsAt.Should().Be(today.AddDays(10));
    }

    [Fact]
    public void CalculateTotalOpenInvoices_ReturnsOnlyOpenInvoices()
    {
        // Arrange
        var invoices = new List<Invoice>
        {
            new() { Amount = 100m, Status = InvoiceStatus.Open },
            new() { Amount = 200m, Status = InvoiceStatus.Paid },
            new() { Amount = 150m, Status = InvoiceStatus.Open }
        };

        // Act
        var result = CostCalculator.CalculateTotalOpenInvoices(invoices);

        // Assert
        result.Should().Be(250m);
    }
}
