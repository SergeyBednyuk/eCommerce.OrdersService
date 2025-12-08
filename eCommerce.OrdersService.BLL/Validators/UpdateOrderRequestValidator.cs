using eCommerce.OrdersService.BLL.DTOs;
using FluentValidation;

namespace eCommerce.OrdersService.BLL.Validators;

public class UpdateOrderRequestValidator : AbstractValidator<UpdateOrderRequest>
{
    public UpdateOrderRequestValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithErrorCode("UserId should not be empty");
        RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId should not be empty");
        RuleFor(x => x.OrderDate).NotEmpty().WithErrorCode("OrderDate should not be empty");
        RuleFor(x => x.OrderItems).NotEmpty().WithErrorCode("OrderItems should not be empty");
    }
}