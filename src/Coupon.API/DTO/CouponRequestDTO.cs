using Microsoft.AspNetCore.Mvc;

namespace Coupon.API.DTO
{
    public class CouponRequestDTO
    {
        [FromHeader(Name = "CouponName")]
        public string CouponName { get; set; }
        [FromHeader(Name = "PageSize")]
        public int PageSize { get; set; }
        [FromHeader(Name = "Page")]
        public int Page { get; set; }
        public ILogger<CouponRequestDTO> Logger { get; set; }
    }
}
