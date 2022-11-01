using Coupon.API.DTO;

namespace Coupon.API.Repository.IRepository
{
    public interface IAuthRepository
    {
        Task<bool> IsUniqueUser(string username);
        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
        Task<LocalUserDTO> Register(RegistrationRequestDTO registrationRequestDTO);
    }
}
