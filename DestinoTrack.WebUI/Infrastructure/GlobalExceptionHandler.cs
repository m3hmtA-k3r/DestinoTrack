using System.Security.Claims;
using Microsoft.AspNetCore.Diagnostics;

namespace DestinoTrack.WebUI.Infrastructure
{
    // Beklenmeyen her hatayı tek yerde yakalar ve ayrıntısıyla günlüğe yazar 
    // Kullanıcıya ayrıntı gösterilmez: false dönünce UseExceptionHandler hata sayfasını çizer
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> _logger) : IExceptionHandler
    {
        public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            // UseExceptionHandler bu metodu çağırmadan önce Request.Path'i "/Home/Error" yapar;
            // hatanın çıktığı asıl adres IExceptionHandlerPathFeature içinde durur
            var path = httpContext.Features.Get<IExceptionHandlerPathFeature>()?.Path ?? httpContext.Request.Path.Value;

            _logger.LogError(exception,
                "Beklenmeyen hata · {Method} {Path} · kullanıcı: {UserId} · iz: {TraceId}",
                httpContext.Request.Method,
                path,
                httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "girişsiz",
                httpContext.TraceIdentifier);

            // false = "cevabı ben yazmadım": ara katman isteği hata sayfasına yönlendirip sayfayı o çizer
            return ValueTask.FromResult(false);
        }

    }
}
