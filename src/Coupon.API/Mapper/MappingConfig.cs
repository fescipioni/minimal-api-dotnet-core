using AutoMapper;

namespace Coupon.API.Mapper
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Models.Coupon, DTO.CouponDTO>().ReverseMap();
            CreateMap<Models.Coupon, DTO.CouponCreateDTO>().ReverseMap();

            CreateMap<Models.LocalUser, DTO.LocalUserDTO>().ReverseMap();
            CreateMap<Models.LocalUser, DTO.RegistrationRequestDTO>().ReverseMap();
            CreateMap<Models.LocalUser, DTO.LoginRequestDTO>().ReverseMap();
        }
    }
}
