using Coupon.API.DTO;
using FluentValidation;

namespace Coupon.API.Validations
{
    public class CouponCreateValidation : AbstractValidator<CouponCreateDTO>
    {
        public CouponCreateValidation()
        {
            RuleFor(dto => dto.Name).NotNull().NotEmpty().WithMessage("Coupon name is required.");
            RuleFor(dto => dto.Percent).InclusiveBetween(1, 100).WithMessage("Percent range is between 1 and 100.");
        }
    }
}
