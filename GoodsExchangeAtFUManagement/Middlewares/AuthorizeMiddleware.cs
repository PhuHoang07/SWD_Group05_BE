﻿using GoodsExchangeAtFUManagement.Repository.Enums;
using GoodsExchangeAtFUManagement.Repository.Repositories.UserRepositories;
using System.Net;
using System.Security.Claims;

namespace GoodsExchangeAtFUManagement.Middlewares
{
    public class AuthorizeMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthorizeMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IUserRepository userRepo)
        {
            try
            {
                var requestPath = context.Request.Path;

                if (requestPath.StartsWithSegments("/api/auth") || requestPath.StartsWithSegments("/api/otp-management"))
                {
                    await _next.Invoke(context);
                    return;
                }

                var userIdentity = context.User.Identity as ClaimsIdentity;
                if (!userIdentity.IsAuthenticated)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return;
                }

                var user = await userRepo.GetSingle(u => u.Id.Equals(Guid.Parse(userIdentity.FindFirst("userid").Value)));

                if (user.Status.Equals(AccountStatusEnums.Inactive.ToString()))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return;
                }

                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsync(ex.ToString());
            }

        }
    }
}
