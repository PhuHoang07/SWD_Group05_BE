﻿using GoodsExchangeAtFUManagement.Service.Ultis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace GoodsExchangeAtFUManagement.Middlewares
{
    public class GlobalExceptionMiddleware : IMiddleware
    {
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(ILogger<GlobalExceptionMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception has occurred");

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = ex is CustomException ? StatusCodes.Status400BadRequest : StatusCodes.Status500InternalServerError;

                var response = new
                {
                    StatusCode = context.Response.StatusCode,
                    Message = ex.Message,
                    Detailed = ex.ToString()
                };

                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
