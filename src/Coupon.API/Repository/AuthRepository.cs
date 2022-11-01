using AutoMapper;
using Coupon.API.Data;
using Coupon.API.DTO;
using Coupon.API.Models;
using Coupon.API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Coupon.API.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        private readonly string _secret;

        public AuthRepository(ApplicationDbContext db, IConfiguration configuration, IMapper mapper)
        {
            _db = db;
            _configuration = configuration;
            _mapper = mapper;

            _secret = _configuration.GetValue<string>("ApiSettings:Secret");
        }

        public async Task<bool> IsUniqueUser(string username)
        {
            LocalUser user = await _db.LocalUsers.FirstOrDefaultAsync(localUser => localUser.UserName.Equals(username));

            return user == null;
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            loginRequestDTO.Password = HashString(loginRequestDTO.Password);

            LocalUser user = await _db.LocalUsers.FirstOrDefaultAsync(localUser => localUser.UserName.Equals(loginRequestDTO.UserName) && localUser.Password.Equals(loginRequestDTO.Password));

            if (user == null) return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            LocalUserDTO userDTO = _mapper.Map<LocalUserDTO>(user);

            LoginResponseDTO loginResponseDTO = new LoginResponseDTO
            {
                User = userDTO,
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            };

            return loginResponseDTO;
        }

        public async Task<LocalUserDTO> Register(RegistrationRequestDTO registrationRequestDTO)
        {
            LocalUser user = _mapper.Map<LocalUser>(registrationRequestDTO);
            user.Role = "admin"; // for purpose of the course it will be static data.
            user.Password = HashString(user.Password);

            await _db.LocalUsers.AddAsync(user);
            await _db.SaveChangesAsync();

            user.Password = string.Empty;

            return _mapper.Map<LocalUserDTO>(user);
        }

        private string HashString(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes("minimal-api-dotnet-core"));
                
                string hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();

                return hash;
            }
        }
    }
}
