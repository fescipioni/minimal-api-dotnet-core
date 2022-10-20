using System.ComponentModel.DataAnnotations;

namespace Coupon.API.DTO
{
    public class CouponCreateDTO
    {
        [Required]
        public string Name { get; set; }
        
        [Required]
        public int Percent { get; set; }
        public bool IsActive { get; set; }
    }
}
