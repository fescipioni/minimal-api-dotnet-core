using AutoMapper;
using Coupon.API.Data;
using Coupon.API.DTO;
using Coupon.API.Mapper;
using FluentValidation;
using FluentValidation.Results;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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

app.MapGet("/api/coupon", (IMapper _mapper, ILogger<Program> _logger) => 
{
    _logger.LogInformation("Getting all coupons");

    IList<CouponDTO> coupons = _mapper.Map<List<CouponDTO>>(CouponStore.couponList);

    return Results.Ok(coupons);
})
.Produces<IList<CouponDTO>>(200); // .WithName("GetCoupon");

app.MapGet("/api/coupon/{id:int}", (IMapper _mapper, int id) =>
{
    var coupon = CouponStore.couponList.FirstOrDefault(coupon => coupon.Id == id);

    if (coupon == null) return Results.BadRequest($"Coupon ID {id} doesn't exist.");

    CouponDTO couponDto = _mapper.Map<CouponDTO>(coupon);

    return Results.Ok(couponDto);
})
.Produces<CouponDTO>(200)
.Produces(400);

app.MapPost("/api/coupon", async (IMapper _mapper, IValidator<CouponCreateDTO> _validator, CouponCreateDTO couponCreateDto) =>
{
    ValidationResult validationResult = await _validator.ValidateAsync(couponCreateDto);

    if (!validationResult.IsValid) return Results.BadRequest(validationResult.Errors.FirstOrDefault().ToString());

    if (CouponStore.couponList.FirstOrDefault(couponStored => couponStored.Name.ToLower().Equals(couponCreateDto.Name.ToLower())) != null) return Results.BadRequest("Coupon name already exists.");

    Coupon.API.Models.Coupon coupon = _mapper.Map<Coupon.API.Models.Coupon>(couponCreateDto);
    coupon.Id = CouponStore.couponList.OrderByDescending(couponStored => couponStored.Id).FirstOrDefault().Id + 1;
    coupon.Created = DateTime.Now;

    CouponStore.couponList.Add(coupon);

    CouponDTO dto = _mapper.Map<CouponDTO>(coupon);

    // if some route has a name, it is possible to return to some specific route by name:
    // return Results.CreatedAtRoute("GetCoupon", new { id = coupon.Id }, coupon);

    return Results.Created($"/api/coupon/{dto.Id}", dto);
})
.Accepts<CouponCreateDTO>("application/json")
.Produces<CouponDTO>(201)
.Produces(400)
.Produces(500);

app.MapPut("/api/coupon", () =>
{

});

app.MapDelete("/api/coupon/{id:int}", (int id) =>
{

});

app.Run();