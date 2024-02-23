using Microsoft.Extensions.DependencyInjection;
using Moiniorell.Application.Abstractions.Services;
using Moiniorell.Infrastructure.Implementations;

namespace Moiniorell.Infrastructure.ServiceRegistration
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructureServices(this IServiceCollection services)
        {
           

            services.AddScoped<ICloudService, CloudService>();
            services.AddScoped<IEmailService, EmailService>();
           // services.AddScoped<IExceptionHandlerService,ExceptionHandlerService>();
        }
    
    }
}
