using Microsoft.Extensions.DependencyInjection;
using Moiniorell.Application.Abstractions.Repositories.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Moiniorell.Application.ServiceRegistration
{
    public static class ServiceRegistration
    {
        public static void AddApplicationServices(this IServiceCollection service)
        {
            service.AddAutoMapper(Assembly.GetExecutingAssembly());
            




        }

    }
}
