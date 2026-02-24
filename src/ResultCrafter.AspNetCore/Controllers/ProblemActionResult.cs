using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using ResultCrafter.AspNetCore.ProblemDetails;
using ResultCrafter.Core.Primitives;

namespace ResultCrafter.AspNetCore.Controllers;

/// <summary>
/// An <see cref="ActionResult"/> that writes an RFC 9457 ProblemDetails response for a
/// given <see cref="Error"/>, routing through <see cref="IProblemDetailsService"/> so
/// that the same enrichment (instance, traceId, requestId) and structured 4xx logging
/// that applies to Minimal API error responses also applies to controller actions.
/// </summary>
/// <remarks>
/// This type is returned by the <see cref="ControllerResultExtensions"/> helper methods
/// on the failure path. Application code typically does not instantiate it directly —
/// let the extension methods do that.
/// </remarks>
public sealed class ProblemActionResult : ActionResult
{
   private readonly Error _error;

   /// <param name="error">The <see cref="Error"/> to convert into a ProblemDetails response.</param>
   public ProblemActionResult(Error error) => _error = error;

   /// <summary>The <see cref="Error"/> this result was constructed from.</summary>
   public Error Error => _error;

   /// <inheritdoc />
   public override async Task ExecuteResultAsync(ActionContext context)
   {
      var httpContext = context.HttpContext;
      var pd = ProblemDetailsBuilder.Build(_error);

      var pds = httpContext.RequestServices.GetRequiredService<IProblemDetailsService>();
      await pds.WriteAsync(new ProblemDetailsContext
      {
         HttpContext = httpContext,
         ProblemDetails = pd
      });
   }
}