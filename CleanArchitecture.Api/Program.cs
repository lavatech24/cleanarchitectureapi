using CleanArchitecture.Api.Filters;
using CleanArchitecture.Application.Interfaces.Services;
using CleanArchitecture.Infrastructure.Data;
using CleanArchitecture.Infrastructure.Repositories;
using CleanArchitecture.Application.Services;
using CleanArchitecture.Application;
using Microsoft.Extensions.DependencyInjection;
using CleanArchitecture.Application.DTOValidations;
using FluentValidation;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CleanArchitecture.Api.Middleware;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options => 
{ 
    options.Filters.Add<ValidationFilter>(); 
    options.Filters.Add<GlobalExceptionFilter>();
})
.ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true);
builder.Configuration.AddJsonFile($"appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();
//Console.WriteLine($"Running in: {builder.Environment.EnvironmentName}");

builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmissionRepository, EmissionRepository>();
builder.Services.AddScoped<IEmissionService, EmissionService>();
builder.Services.AddScoped<IIPWhitelistRepository, CachedIPWhitelistRepository>();
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(cfg =>
    {
        cfg.RequireHttpsMetadata = false;
        cfg.SaveToken = true;
        cfg.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = builder.Configuration.GetValue<String>("Tokens:JwtIssuer"),
            ValidAudience = builder.Configuration.GetValue<String>("Tokens:Audience"),
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<String>("Tokens:JwtKey"))),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero // remove delay of token when expire
        };
		cfg.Events = new JwtBearerEvents
		{
			OnAuthenticationFailed = ctx =>
			{
				Console.WriteLine($"Auth failed: {ctx.Exception.Message}");
				return Task.CompletedTask;
			},
			OnChallenge = ctx =>
			{
				Console.WriteLine("Auth challenge triggered");
				return Task.CompletedTask;
			}
		};

	});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddMemoryCache(); //Used in RateLimitResourceFilter
builder.Services.AddSqlServerDatabase(builder.Configuration);
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());
builder.Services.AddValidatorsFromAssemblyContaining<CreateApiUserValidator>();
builder.Services.AddScoped<RateLimitResourceFilter>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => options.WithTitle("My Api").WithTheme(ScalarTheme.BluePlanet).WithSidebar(true));
    app.UseSwaggerUi(options => options.DocumentPath = "openapi/v1.json");
}

app.UseHttpsRedirection();

app.UseMiddleware<IPWhitelistMiddleware>();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<CleanArchitectureContext>();
    context.Database.EnsureCreated();
    CleanArchitectureContextFactory.Initialize(context, builder.Configuration);
}

app.Run();
