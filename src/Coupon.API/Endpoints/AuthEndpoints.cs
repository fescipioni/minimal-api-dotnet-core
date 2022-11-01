using AutoMapper;
using Coupon.API.DTO;
using Coupon.API.Repository.IRepository;
using FluentValidation;
using FluentValidation.Results;
using System.Net;

namespace Coupon.API.Endpoints
{
    public static class AuthEndpoints
    {
        public static void ConfigureAuthEndpoints(this WebApplication app)
        {
            app.MapPost("/api/login", Login).Accepts<LoginRequestDTO>("application/json").Produces<ApiResponse>(200).Produces(400);

            app.MapPost("/api/register", Register).Accepts<RegistrationRequestDTO>("application/json").Produces<ApiResponse>(200).Produces(400);
        }

        private async static Task<IResult> Register(IAuthRepository _authRepository, RegistrationRequestDTO registrationRequestDTO)
        {
            ApiResponse response = new()
            {
                StatusCode = HttpStatusCode.BadRequest
            };

            if (!await _authRepository.IsUniqueUser(registrationRequestDTO.UserName))
            {
                response.Errors.Add("Username already exists!");

                return Results.BadRequest(response);
            }

            LocalUserDTO userDTO = await _authRepository.Register(registrationRequestDTO);

            if (userDTO == null || string.IsNullOrEmpty(userDTO.UserName))
            {
                response.Errors.Add("An error has occurred during the registration!");

                return Results.BadRequest(response);
            }

            response.Result = userDTO;
            response.StatusCode = HttpStatusCode.OK;

            return Results.Ok(response);
        }

        private async static Task<IResult> Login(IAuthRepository _authRepository, LoginRequestDTO loginRequestDTO)
        {
            ApiResponse response = new()
            {
                StatusCode = HttpStatusCode.BadRequest
            };

            LoginResponseDTO loginResponseDTO = await _authRepository.Login(loginRequestDTO);

            if (loginResponseDTO == null)
            {
                response.Errors.Add("Username or password is incorrect!");

                return Results.BadRequest(response);
            }

            response.Result = loginResponseDTO;
            response.StatusCode = HttpStatusCode.OK;
            
            return Results.Ok(response);
        }
    }
}
