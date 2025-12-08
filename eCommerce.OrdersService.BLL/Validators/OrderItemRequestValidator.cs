using eCommerce.OrdersService.BLL.DTOs;
using FluentValidation;

namespace eCommerce.OrdersService.BLL.Validators;

public class OrderItemRequestValidator : AbstractValidator<OrderItemRequest>
{
    public OrderItemRequestValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty().WithErrorCode("Product Id cannot be empty");
        RuleFor(x => x.Quantity).GreaterThan(0).WithErrorCode("Quantity should be greater than zero");
        RuleFor(x => x.UnitPrice).GreaterThan(0).WithErrorCode("UnitPrice should be greater than zero");
    }
}