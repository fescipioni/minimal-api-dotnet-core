namespace Coupon.API.Data
{
    public static class CouponStore
    {
        public static List<Models.Coupon> couponList = new List<Models.Coupon>
        {
            new Models.Coupon { Id = 1, Name = "10OFF", Percent = 10, IsActive = true, Created = new DateTime(2022, 8, 3) },
            new Models.Coupon { Id = 2, Name = "20OFF", Percent = 10, IsActive = false, Created = new DateTime(2022, 9, 3) }
        };
    }
}
