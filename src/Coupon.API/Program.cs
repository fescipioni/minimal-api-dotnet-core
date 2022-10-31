using AutoMapper;
using Coupon.API.Data;
using Coupon.API.DTO;
using Coupon.API.Mapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);
builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/api/coupon", (ApplicationDbContext _db, IMapper _mapper, ILogger<Program> _logger) => 
{
    _logger.LogInformation("Getting all coupons");

    IList<CouponDTO> coupons = _mapper.Map<List<CouponDTO>>(_db.Coupons);

    ApiResponse response = new()
    {
        Result = coupons,
        StatusCode = HttpStatusCode.OK
    };

    return Results.Ok(response);
})
.Produces<ApiResponse>(200); // .WithName("GetCoupon");

app.MapGet("/api/coupon/{id:int}", async (ApplicationDbContext _db, IMapper _mapper, int id) =>
{
    ApiResponse response = new()
    {
        StatusCode = HttpStatusCode.NotFound
    };

    var coupon = await _db.Coupons.FirstOrDefaultAsync(coupon => coupon.Id == id);

    if (coupon == null)
    {
        response.Errors.Add($"Coupon ID {id} doesn't exist.");
        
        return Results.NotFound(response);
    }

    CouponDTO couponDto = _mapper.Map<CouponDTO>(coupon);

    response.StatusCode = HttpStatusCode.OK;
    response.Result = couponDto;

    return Results.Ok(response);
})
.Produces<ApiResponse>(200)
.Produces(400)
.Produces(404);

app.MapPost("/api/coupon", async (ApplicationDbContext _db, IMapper _mapper, IValidator<CouponCreateDTO> _validator, CouponCreateDTO couponCreateDto) =>
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

    if (await _db.Coupons.FirstOrDefaultAsync(couponStored => couponStored.Name.ToLower().Equals(couponCreateDto.Name.ToLower())) != null) 
    {
        response.Errors.Add("Coupon name already exists.");

        return Results.BadRequest(response); 
    }

    Coupon.API.Models.Coupon coupon = _mapper.Map<Coupon.API.Models.Coupon>(couponCreateDto);
    coupon.Created = DateTime.Now;

    _db.Coupons.Add(coupon);
    await _db.SaveChangesAsync();

    CouponDTO dto = _mapper.Map<CouponDTO>(coupon);

    response.StatusCode = HttpStatusCode.OK;
    response.Result = dto;
    
    // if some route has a name, it is possible to return to some specific route by name:
    // return Results.CreatedAtRoute("GetCoupon", new { id = coupon.Id }, coupon);

    return Results.Created($"/api/coupon/{dto.Id}", response);
})
.Accepts<CouponCreateDTO>("application/json")
.Produces<ApiResponse>(201)
.Produces(400)
.Produces(500);

app.MapPut("/api/coupon/{id:int}", async (ApplicationDbContext _db, IValidator<CouponCreateDTO> _validator, int id, CouponUpdateDTO couponUpdateDto) =>
{
    ApiResponse response = new();

    ValidationResult validationResult = await _validator.ValidateAsync(couponUpdateDto);

    if (!validationResult.IsValid)
    {
        response.Errors = validationResult.Errors.Select(error => error.ToString()).ToList();
        response.StatusCode = HttpStatusCode.BadRequest;

        return Results.BadRequest(response);
    }

    var coupon = await _db.Coupons.FirstOrDefaultAsync(coupon => coupon.Id == id);

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

    await _db.SaveChangesAsync();

    return Results.NoContent();
})
.Produces(204)
.Produces<ApiResponse>(400)
.Produces<ApiResponse>(404);

app.MapDelete("/api/coupon/{id:int}", async (ApplicationDbContext _db, int id) =>
{
    ApiResponse response = new();

    if (id < 1)
    {
        response.Errors.Add("Coupon ID is not valid.");
        response.StatusCode = HttpStatusCode.BadRequest;

        return Results.BadRequest(response);
    }

    var coupon = await _db.Coupons.FirstOrDefaultAsync(coupon => coupon.Id == id);

    if (coupon == null)
    {
        response.Errors.Add($"Coupon ID {id} doesn't exist.");
        response.StatusCode = HttpStatusCode.NotFound;

        return Results.NotFound(response);
    }

    _db.Coupons.Remove(coupon);
    await _db.SaveChangesAsync();

    return Results.NoContent();
})
.Produces(204)
.Produces<ApiResponse>(400)
.Produces<ApiResponse>(404);

app.Run();