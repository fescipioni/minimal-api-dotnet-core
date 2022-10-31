using Microsoft.EntityFrameworkCore;

namespace Coupon.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Models.Coupon> Coupons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Models.Coupon>().HasData(
                new Models.Coupon { Id = 1, Name = "10OFF", Percent = 10, IsActive = true, Created = new DateTime(2022, 8, 3) },
                new Models.Coupon { Id = 2, Name = "20OFF", Percent = 10, IsActive = false, Created = new DateTime(2022, 9, 3) }
            );
        }
    }
}
