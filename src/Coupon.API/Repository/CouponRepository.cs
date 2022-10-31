using Coupon.API.Data;
using Coupon.API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Coupon.API.Repository
{
    public class CouponRepository : ICouponRepository
    {
        private readonly ApplicationDbContext _db;

        public CouponRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task CreateAsync(Models.Coupon coupon)
        {
            await _db.Coupons.AddAsync(coupon);

            await SaveAsync();
        }

        public async Task<ICollection<Models.Coupon>> GetAllAsync()
        {
            return await _db.Coupons.ToListAsync();
        }

        public async Task<Models.Coupon> GetAsync(int id)
        {
            return await _db.Coupons.FirstOrDefaultAsync(coupon => coupon.Id == id);
        }

        public async Task<Models.Coupon> GetAsync(string couponName)
        {
            return await _db.Coupons.FirstOrDefaultAsync(coupon => coupon.Name.ToLower().Equals(couponName.ToLower()));
        }

        public async Task RemoveAsync(Models.Coupon coupon)
        {
            _db.Coupons.Remove(coupon);

            await SaveAsync();
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Models.Coupon coupon)
        {
            _db.Coupons.Update(coupon);

            await SaveAsync();
        }
    }
}
