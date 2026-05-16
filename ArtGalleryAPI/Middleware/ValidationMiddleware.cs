using ArtGalleryAPI.DTOs;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace ArtGalleryAPI.Middleware
{
    public class ValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ValidationMiddleware> _logger;

        public ValidationMiddleware(RequestDelegate next, ILogger<ValidationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            // Handle validation errors from ModelState
            if (context.Response.StatusCode == 400)
            {
                // Validation errors are handled by the framework
            }
        }
    }

    public class ValidationBehavior
    {
    }
}
