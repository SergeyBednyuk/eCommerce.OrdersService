using eCommerce.OrdersService.BLL.DTOs;
using FluentValidation;

namespace eCommerce.OrdersService.BLL.Validators;

public class GetOrdersQueryValidator : AbstractValidator<GetOrdersQuery>
{
    public GetOrdersQueryValidator()
    {
        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("PageSize  must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("PageSize  must be less than 100");
        
        RuleFor(x => x.PageNumber).GreaterThan(0).WithMessage("PageNumber  must be greater than 0");
        
        //Guids
        RuleFor(x => x.OrderId)
            .NotEqual(Guid.Empty).WithMessage("OrderId cannot be empty")
            .When(x => x.OrderId.HasValue);
        
        RuleFor(x => x.UserId)
            .NotEqual(Guid.Empty).WithMessage("UserId cannot be empty")
            .When(x => x.UserId.HasValue);

        //Price range
        RuleFor(x => x.MinTotal)
            .NotEqual(0).WithMessage("MinTotal  must be greater than 0")
            .When(x => x.MinTotal.HasValue);

        RuleFor(x => x.MaxTotal)
            .GreaterThanOrEqualTo(0).WithMessage("MaxTotal  must be greater than 0")
            .GreaterThan(x => x.MinTotal).WithMessage("MaxTotal must be greater than 0 or MinTotal")
            .When(x => x.MaxTotal.HasValue && x.MinTotal.HasValue);
        
        RuleFor(x=>x.ToDate)
            .GreaterThanOrEqualTo(x => x.FromDate).WithMessage("ToDate  must be greater than FromDate")
            .When(x => x.ToDate.HasValue && x.FromDate.HasValue);
    }
}