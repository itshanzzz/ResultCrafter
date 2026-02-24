using ResultCrafter.AspNetCore.ProblemDetails;
using ResultCrafter.Core.Primitives;

namespace ResultCrafter.Tests.AspNetCore;

public sealed class HttpErrorCatalogTests
{
   // ── Status codes ───────────────────────────────────────────────────────────

   [Theory]
   [InlineData(ErrorType.BadRequest, 400)]
   [InlineData(ErrorType.Unauthorized, 401)]
   [InlineData(ErrorType.Forbidden, 403)]
   [InlineData(ErrorType.NotFound, 404)]
   [InlineData(ErrorType.Conflict, 409)]
   [InlineData(ErrorType.ConcurrencyConflict, 409)]
   public void Status_ReturnsCorrectHttpStatusCode(ErrorType type, int expectedStatus)
   {
      Assert.Equal(expectedStatus, HttpErrorCatalog.Status(type));
   }

   // ── Titles ─────────────────────────────────────────────────────────────────

   [Theory]
   [InlineData(ErrorType.BadRequest, "bad_request")]
   [InlineData(ErrorType.Unauthorized, "unauthorized")]
   [InlineData(ErrorType.Forbidden, "forbidden")]
   [InlineData(ErrorType.NotFound, "not_found")]
   [InlineData(ErrorType.Conflict, "conflict")]
   [InlineData(ErrorType.ConcurrencyConflict, "concurrency_conflict")]
   public void Title_ReturnsCorrectTitle(ErrorType type, string expectedTitle)
   {
      Assert.Equal(expectedTitle, HttpErrorCatalog.Title(type));
   }

   // ── Detail resolution ─────────────────────────────────────────────────────

   [Fact]
   public void ResolveDetail_WhenErrorHasDetail_ReturnsErrorDetail()
   {
      var error = Error.NotFound("Order 42 does not exist.");
      Assert.Equal("Order 42 does not exist.", HttpErrorCatalog.ResolveDetail(error));
   }

   [Fact]
   public void ResolveDetail_WhenErrorHasNoDetail_ReturnsCatalogDefault()
   {
      var error = Error.NotFound();
      Assert.Equal("resource_not_found", HttpErrorCatalog.ResolveDetail(error));
   }

   [Fact]
   public void ResolveDetail_BadRequestWithNoDetail_ReturnsCatalogDefault()
   {
      // BadRequest via dictionary has no explicit detail
      var error = Error.BadRequest(new Dictionary<string, string[]>
      {
         ["name"] = ["Required."]
      });
      Assert.Equal("the_request_was_invalid_or_cannot_be_otherwise_served",
         HttpErrorCatalog.ResolveDetail(error));
   }
}