namespace Coupon.API.DTO
{
    public class CouponDTO : CouponCreateDTO
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
    }
}
