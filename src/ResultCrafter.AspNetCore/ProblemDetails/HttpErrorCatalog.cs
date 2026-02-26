using ResultCrafter.Core.Primitives;

namespace ResultCrafter.AspNetCore.ProblemDetails;

/// <summary>
///    Maps <see cref="ErrorType" /> values to HTTP status codes, ProblemDetails titles,
///    and default detail messages.
/// </summary>
public static class HttpErrorCatalog
{
   /// <summary>Maps <paramref name="type"/> to its RFC 9457 HTTP status code.</summary>
   public static int Status(ErrorType type)
   {
      return type switch
      {
         ErrorType.BadRequest => 400,
         ErrorType.Unauthorized => 401,
         ErrorType.Forbidden => 403,
         ErrorType.NotFound => 404,
         ErrorType.Conflict => 409,
         ErrorType.ConcurrencyConflict => 409,
         _ => 400
      };
   }

   /// <summary>Maps <paramref name="type"/> to its snake_case ProblemDetails <c>title</c> string.</summary>
   public static string Title(ErrorType type)
   {
      return type switch
      {
         ErrorType.NotFound => "not_found",
         ErrorType.Conflict => "conflict",
         ErrorType.Unauthorized => "unauthorized",
         ErrorType.Forbidden => "forbidden",
         ErrorType.BadRequest => "bad_request",
         ErrorType.ConcurrencyConflict => "concurrency_conflict",
         _ => "bad_request"
      };
   }

   public static string DefaultDetail(ErrorType type)
   {
      return type switch
      {
         ErrorType.BadRequest => "the_request_was_invalid_or_cannot_be_otherwise_served",
         ErrorType.NotFound => "resource_not_found",
         ErrorType.Conflict => "conflict",
         ErrorType.ConcurrencyConflict => "concurrency_conflict",
         ErrorType.Unauthorized => "unauthorized",
         ErrorType.Forbidden => "forbidden",
         _ => "bad_request"
      };
   }

   /// <summary>Returns the error's own detail message, falling back to the catalog default.</summary>
   public static string ResolveDetail(Error error)
   {
      return error.Detail ?? DefaultDetail(error.Type);
   }
}