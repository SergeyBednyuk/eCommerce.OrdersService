using eCommerce.OrdersService.BLL.DTOs;
using FluentValidation;

namespace eCommerce.OrdersService.BLL.Validators;

public class AddOrderRequestValidator : AbstractValidator<AddOrderRequest>
{
    public AddOrderRequestValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId should not be empty");
        RuleFor(x => x.OrderDate).NotEmpty().WithErrorCode("OrderDate should not be empty");
        RuleFor(x => x.OrderItems).NotEmpty().WithErrorCode("OrderItems should not be empty");
    }
}