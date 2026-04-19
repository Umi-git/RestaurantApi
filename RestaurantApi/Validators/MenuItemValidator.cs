using FluentValidation;
using RestaurantApi.DTOs;

namespace RestaurantApi.Validators;

public class CreateMenuItemValidator : AbstractValidator<CreateMenuItemDto>
{
    public CreateMenuItemValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Category).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Price).GreaterThan(0).LessThan(1000);
        RuleFor(x => x.Description).MaximumLength(500);
    }
}