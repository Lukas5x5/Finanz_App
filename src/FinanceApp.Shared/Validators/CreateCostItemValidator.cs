namespace FinanceApp.Shared.Validators;

using FluentValidation;
using FinanceApp.Shared.DTOs;

public class CreateCostItemValidator : AbstractValidator<CreateCostItemDto>
{
    public CreateCostItemValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name ist erforderlich")
            .MaximumLength(200).WithMessage("Name darf maximal 200 Zeichen lang sein");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Betrag muss größer als 0 sein");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Währung ist erforderlich")
            .Length(3).WithMessage("Währung muss ein 3-stelliger Code sein (z.B. EUR)");

        RuleFor(x => x.BindingEndsAt)
            .NotNull().When(x => x.HasBinding)
            .WithMessage("Bindungsende ist erforderlich, wenn eine Bindung besteht");

        RuleFor(x => x.Category)
            .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.Category))
            .WithMessage("Kategorie darf maximal 100 Zeichen lang sein");
    }
}
