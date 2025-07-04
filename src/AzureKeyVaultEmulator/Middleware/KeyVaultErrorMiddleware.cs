﻿using System.Net;

namespace AzureKeyVaultEmulator.Middleware
{
    public class KeyVaultErrorMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception e)
            {
                var req = context.Request;

                var errorResponse = new KeyVaultError
                {
                    Code = "Failed to perform request into Azure Key Vault Emulator",
                    InnerError = e.InnerException?.Message ?? string.Empty,
                    Message = e.Message
                };

                var status = e is MissingItemException ? HttpStatusCode.NotFound : HttpStatusCode.BadRequest;

                context.Response.StatusCode = (int)status;
                await context.Response.WriteAsJsonAsync(errorResponse);

                return;
            }
        }
    }
}
