﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moiniorell.Application.Abstractions.Repositories;
using Moiniorell.Application.Abstractions.Services;
using Moiniorell.Domain.Models;
using Moiniorell.Persistence.DAL;
using Moiniorell.Persistence.Implementations.Repositories;
using Moiniorell.Persistence.Implementations.Services;
using System.Reflection;

namespace Moiniorell.Persistence.ServiceRegistration
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(configuration.GetConnectionString("Default"), b => b.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName)));

            AddIdentity(services);

            services.AddScoped<IAuthService, AuthService>();
            services.AddHttpContextAccessor();
            services.AddScoped<IFollowRepository, FollowRepository>();


        }

        private static void AddIdentity(IServiceCollection services)
        {
            services.AddIdentity<AppUser, IdentityRole>(opt =>
            {
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequiredLength = 8;


                opt.User.RequireUniqueEmail = true;

                opt.Lockout.AllowedForNewUsers = true;
                opt.Lockout.MaxFailedAccessAttempts = 5;
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);

            }).AddDefaultTokenProviders().AddEntityFrameworkStores<AppDbContext>();
        }
    }
}