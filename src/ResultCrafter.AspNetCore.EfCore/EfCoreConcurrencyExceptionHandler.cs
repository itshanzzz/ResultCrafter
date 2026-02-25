using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ResultCrafter.AspNetCore.ProblemDetails;
using ResultCrafter.Core.Primitives;

namespace ResultCrafter.AspNetCore.EfCore;

internal sealed class EfCoreConcurrencyExceptionHandler(ILogger<EfCoreConcurrencyExceptionHandler> logger)
   : IExceptionHandler
{
   public async ValueTask<bool> TryHandleAsync(HttpContext httpContext,
      Exception exception,
      CancellationToken cancellationToken)
   {
      if (exception is not DbUpdateConcurrencyException)
      {
         return false;
      }

      if (httpContext.Response.HasStarted)
      {
         ResultCrafterLogger.LogResponseAlreadyStarted(
            logger,
            httpContext.Request.Method,
            httpContext.Request.Path.Value ?? string.Empty);

         return false;
      }

      var error = Error.ConcurrencyConflict();

      ResultCrafterLogger.LogClientError(
         logger,
         LogLevel.Warning,
         HttpErrorCatalog.Status(error.Type),
         HttpErrorCatalog.Title(error.Type),
         HttpErrorCatalog.ResolveDetail(error),
         error.Type.ToString(),
         httpContext.Request.Method,
         httpContext.Request.Path.Value ?? string.Empty,
         ProblemDetailsEnricher.GetInstance(httpContext),
         ProblemDetailsEnricher.GetTraceId(httpContext),
         httpContext.TraceIdentifier);

      var pd = ProblemDetailsBuilder.Build(error);

      var pds = httpContext.RequestServices.GetRequiredService<IProblemDetailsService>();
      return await pds.TryWriteAsync(new ProblemDetailsContext
      {
         HttpContext = httpContext,
         ProblemDetails = pd,
         Exception = exception
      });
   }
}