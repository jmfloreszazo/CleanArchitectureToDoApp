﻿using Microsoft.EntityFrameworkCore;
using ToDoApp.Domain.Interfaces;
using ToDoApp.Persistence.Repository;
using ToDoApp.Persistence;
using ToDoApp.Infrastructure.EmailService;
using ToDoApp.Domain.Email;
using Microsoft.AspNetCore.Identity;
using ToDoApp.Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ToDoApp.Infrastructure.AuthService;

namespace ToDoApp.API.Extensions;

public static class ServiceExtensions
{
    public static void AddApplicationDependencies(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Application.AssemblyReference).Assembly));
    }

    public static void AddPersistenceDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ToDoDbContext>(opts =>
            opts.UseSqlServer(configuration.GetConnectionString("sqlConnection"),
            b => b.MigrationsAssembly("ToDoApp.Persistence")));

        services.AddScoped<IToDoRepository, SqlToDoRepository>();
    }

    public static void AddInfrastructureDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        var emailConfig = configuration
            .GetSection("EmailConfiguration")
            .Get<EmailConfiguration>();
        services.AddSingleton(emailConfig!);
        services.AddScoped<IEmailSender, EmailSender>();

        services.AddScoped<IAuthenticationService, AuthenticationService>();

        services.AddIdentity<User, IdentityRole>(o =>
        {
            o.Password.RequireDigit = true;
            o.Password.RequireLowercase = false;
            o.Password.RequireUppercase = false;
            o.Password.RequireNonAlphanumeric = false;
            o.Password.RequiredLength = 10;
            o.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<ToDoDbContext>()
        .AddDefaultTokenProviders();

        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["YourSecret"]; //you need to have this one set on your PC with a valid length

        services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtSettings["validIssuer"],
                ValidAudience = jwtSettings["validAudience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
            };
        });
    }
}
