using AutoMapper;
using Coupon.API.DTO;
using Coupon.API.Repository.IRepository;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using System.Net;

namespace Coupon.API.Endpoints
{
    public static class CouponEndpoints
    {
        public static void ConfigureCouponEndpoints(this WebApplication app)
        {
            app.MapGet("/api/coupon", GetAllCoupons).Produces<ApiResponse>(200) // .WithName("GetCoupon");
            .RequireAuthorization("AdminOnly"); 

            app.MapGet("/api/coupon/{id:int}", GetCoupon)
            .Produces<ApiResponse>(200)
            .Produces(400)
            .Produces(404);

            app.MapPost("/api/coupon", CreateCoupon)
            .Accepts<CouponCreateDTO>("application/json")
            .Produces<ApiResponse>(201)
            .Produces(400)
            .Produces(500);

            app.MapPut("/api/coupon/{id:int}", UpdateCoupon)
            .Accepts<CouponUpdateDTO>("application/json")
            .Produces(204)
            .Produces<ApiResponse>(400)
            .Produces<ApiResponse>(404);

            app.MapDelete("/api/coupon/{id:int}", RemoveCoupon)
            .Produces(204)
            .Produces<ApiResponse>(400)
            .Produces<ApiResponse>(404);
        }

        private async static Task<IResult> GetAllCoupons(ICouponRepository _couponRepository, IMapper _mapper, ILogger<Program> _logger)
        {
            _logger.LogInformation("Getting all coupons");

            IList<CouponDTO> coupons = _mapper.Map<List<CouponDTO>>(await _couponRepository.GetAllAsync());

            ApiResponse response = new()
            {
                Result = coupons,
                StatusCode = HttpStatusCode.OK
            };

            return Results.Ok(response);
        }

        [Authorize(Policy = "AdminOnly")]
        private async static Task<IResult> GetCoupon(ICouponRepository _couponRepository, IMapper _mapper, int id)
        {
            ApiResponse response = new()
            {
                StatusCode = HttpStatusCode.NotFound
            };

            var coupon = await _couponRepository.GetAsync(id);

            if (coupon == null)
            {
                response.Errors.Add($"Coupon ID {id} doesn't exist.");

                return Results.NotFound(response);
            }

            CouponDTO couponDto = _mapper.Map<CouponDTO>(coupon);

            response.StatusCode = HttpStatusCode.OK;
            response.Result = couponDto;

            return Results.Ok(response);
        }

        [Authorize(Roles = "admin")]
        private async static Task<IResult> CreateCoupon(ICouponRepository _couponRepository, IMapper _mapper, IValidator<CouponCreateDTO> _validator, CouponCreateDTO couponCreateDto)
        {
            ApiResponse response = new()
            {
                StatusCode = HttpStatusCode.BadRequest
            };

            ValidationResult validationResult = await _validator.ValidateAsync(couponCreateDto);

            if (!validationResult.IsValid)
            {
                response.Errors = validationResult.Errors.Select(error => error.ToString()).ToList();

                return Results.BadRequest(response);
            }

            if (await _couponRepository.GetAsync(couponCreateDto.Name) != null)
            {
                response.Errors.Add("Coupon name already exists.");

                return Results.BadRequest(response);
            }

            Coupon.API.Models.Coupon coupon = _mapper.Map<Coupon.API.Models.Coupon>(couponCreateDto);
            coupon.Created = DateTime.Now;

            await _couponRepository.CreateAsync(coupon);

            CouponDTO dto = _mapper.Map<CouponDTO>(coupon);

            response.StatusCode = HttpStatusCode.OK;
            response.Result = dto;

            // if some route has a name, it is possible to return to some specific route by name:
            // return Results.CreatedAtRoute("GetCoupon", new { id = coupon.Id }, coupon);

            return Results.Created($"/api/coupon/{dto.Id}", response);
        }

        [Authorize]
        private async static Task<IResult> UpdateCoupon(ICouponRepository _couponRepository, IValidator<CouponCreateDTO> _validator, int id, CouponUpdateDTO couponUpdateDto)
        {
            ApiResponse response = new();

            ValidationResult validationResult = await _validator.ValidateAsync(couponUpdateDto);

            if (!validationResult.IsValid)
            {
                response.Errors = validationResult.Errors.Select(error => error.ToString()).ToList();
                response.StatusCode = HttpStatusCode.BadRequest;

                return Results.BadRequest(response);
            }

            var coupon = await _couponRepository.GetAsync(id);

            if (coupon == null)
            {
                response.Errors.Add($"Coupon ID {id} doesn't exist.");
                response.StatusCode = HttpStatusCode.NotFound;

                return Results.NotFound(response);
            }

            coupon.Name = couponUpdateDto.Name;
            coupon.Percent = couponUpdateDto.Percent;
            coupon.IsActive = couponUpdateDto.IsActive;
            coupon.Updated = DateTime.Now;

            await _couponRepository.UpdateAsync(coupon);

            return Results.NoContent();
        }

        [Authorize]
        private async static Task<IResult> RemoveCoupon(ICouponRepository _couponRepository, int id)
        {
            ApiResponse response = new();

            if (id < 1)
            {
                response.Errors.Add("Coupon ID is not valid.");
                response.StatusCode = HttpStatusCode.BadRequest;

                return Results.BadRequest(response);
            }

            var coupon = await _couponRepository.GetAsync(id);

            if (coupon == null)
            {
                response.Errors.Add($"Coupon ID {id} doesn't exist.");
                response.StatusCode = HttpStatusCode.NotFound;

                return Results.NotFound(response);
            }

            await _couponRepository.RemoveAsync(coupon);

            return Results.NoContent();
        }
    }
}
