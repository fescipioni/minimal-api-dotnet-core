namespace Coupon.API.Repository.IRepository
{
    public interface ICouponRepository
    {
        Task<ICollection<Models.Coupon>> GetAllAsync();
        Task<Models.Coupon> GetAsync(int id);
        Task<Models.Coupon> GetAsync(string couponName);
        Task CreateAsync(Models.Coupon coupon);
        Task UpdateAsync(Models.Coupon coupon);
        Task RemoveAsync(Models.Coupon coupon);
        Task SaveAsync();
    }
}
