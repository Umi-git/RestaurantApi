using FluentValidation;
using RestaurantApi.DTOs;

namespace RestaurantApi.Validators;

public class CreateOrderValidator : AbstractValidator<CreateOrderDto>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.Items).NotEmpty().WithMessage("Order must have at least one item.");
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.MenuItemId).GreaterThan(0);
            item.RuleFor(i => i.Quantity).GreaterThan(0).LessThanOrEqualTo(50);
        });
    }
}