using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System;

namespace Presentation.Helpers
{
    public static class BasketResolver
    {
        private const string BasketCookieName = "BasketId";
        private const int CookieExpirationDays = 30;

        public static string GetOrCreateBasketId(HttpContext context)
        {
            var basketId = GetBasketId(context);

            if (string.IsNullOrEmpty(basketId))
            {
                basketId = Guid.NewGuid().ToString();
                SetBasketId(context, basketId);
            }

            return basketId;
        }

        public static void SetBasketId(HttpContext context, string basketId)
        {
            var environment = context.RequestServices.GetService(typeof(IHostEnvironment)) as IHostEnvironment;
            var isDevelopment = environment?.IsDevelopment() ?? false;

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = !isDevelopment, // Allow non-HTTPS in development
                SameSite = isDevelopment ? SameSiteMode.Lax : SameSiteMode.Strict, // More permissive in development
                Expires = DateTime.UtcNow.AddDays(CookieExpirationDays)
            };

            context.Response.Cookies.Append(BasketCookieName, basketId, cookieOptions);
        }

        public static string? GetBasketId(HttpContext context)
        {
            context.Request.Cookies.TryGetValue(BasketCookieName, out var basketId);
            return basketId;
        }

        public static void ClearBasketId(HttpContext context)
        {
            var environment = context.RequestServices.GetService(typeof(IHostEnvironment)) as IHostEnvironment;
            var isDevelopment = environment?.IsDevelopment() ?? false;

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = !isDevelopment,
                SameSite = isDevelopment ? SameSiteMode.Lax : SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(-1) // Expire immediately
            };

            context.Response.Cookies.Delete(BasketCookieName, cookieOptions);
        }
    }
}
