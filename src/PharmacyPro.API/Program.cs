using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PharmacyPro.API.Extensions;
using PharmacyPro.API.Middlewares;
using PharmacyPro.Application;
using PharmacyPro.Application.Common;
using PharmacyPro.Application.Responses;
using PharmacyPro.Infrastructure;
using PharmacyPro.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<BusinessSettings>(builder.Configuration.GetSection(BusinessSettings.SectionName));
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))
    .ConfigureApiBehaviorOptions(o =>
    {
        // Хатогиҳои validation -> Response<T> формат
        o.InvalidModelStateResponseFactory = ctx =>
        {
            var errors = ctx.ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return new ObjectResult(Response<object>.Fail("Маълумот нодуруст аст", 400, errors)) { StatusCode = 400 };
        };
    });

builder.Services.AddSwaggerWithJwt();

builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
{
    var origins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ?? [];
    p.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod();
}));

// Ҳимоя аз brute-force: 10 request дар як дақиқа
builder.Services.AddRateLimiter(o =>
{
    o.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    o.AddFixedWindowLimiter("auth", opt =>
    {
        opt.PermitLimit = 10;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueLimit = 0;
    });
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseCors();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await DbInitializer.InitializeAsync(app.Services);

app.Run();
