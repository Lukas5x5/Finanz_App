namespace FinanceApp.Shared.Validators;

using FluentValidation;
using FinanceApp.Shared.DTOs;

public class CreateInvoiceValidator : AbstractValidator<CreateInvoiceDto>
{
    public CreateInvoiceValidator()
    {
        RuleFor(x => x.Vendor)
            .NotEmpty().WithMessage("Lieferant ist erforderlich")
            .MaximumLength(200).WithMessage("Lieferant darf maximal 200 Zeichen lang sein");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Betrag muss größer als 0 sein");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Währung ist erforderlich")
            .Length(3).WithMessage("Währung muss ein 3-stelliger Code sein (z.B. EUR)");

        RuleFor(x => x.DueAt)
            .NotEmpty().WithMessage("Fälligkeitsdatum ist erforderlich");

        RuleFor(x => x.Category)
            .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.Category))
            .WithMessage("Kategorie darf maximal 100 Zeichen lang sein");
    }
}
