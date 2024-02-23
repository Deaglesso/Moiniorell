using Microsoft.AspNetCore.Http;
using Moiniorell.Application.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moiniorell.Infrastructure.Implementations
{
    public class ExceptionHandlerService:IExceptionHandlerService
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlerService(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception e)
            {
                string encodedErrorMessage = Uri.EscapeDataString(e.Message);
                string errorPath = $"/Home/ErrorPage?error={encodedErrorMessage}";
                context.Response.Redirect(errorPath);
            }
        }
    }
}
