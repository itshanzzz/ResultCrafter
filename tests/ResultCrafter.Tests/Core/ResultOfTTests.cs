using ResultCrafter.Core.Primitives;

namespace ResultCrafter.Tests.Core;

public sealed class ResultOfTTests
{
   // ── Ok ────────────────────────────────────────────────────────────────────

   [Fact]
   public void Ok_IsSuccess_IsTrue()
   {
      var result = Result<string>.Ok("hello");
      Assert.True(result.IsSuccess);
   }

   [Fact]
   public void Ok_CarriesValue()
   {
      var result = Result<string>.Ok("hello");
      Assert.Equal("hello", result.Value);
   }

   [Fact]
   public void Ok_KindIsOk()
   {
      var result = Result<string>.Ok("hello");
      Assert.Equal(SuccessKind.Ok, result.Kind);
   }

   [Fact]
   public void Ok_ErrorIsNull()
   {
      var result = Result<string>.Ok("hello");
      Assert.Null(result.Error);
   }

   [Fact]
   public void Ok_LocationIsNull()
   {
      var result = Result<string>.Ok("hello");
      Assert.Null(result.Location);
   }

   // ── Created ───────────────────────────────────────────────────────────────

   [Fact]
   public void Created_IsSuccess_IsTrue()
   {
      var result = Result<string>.Created("/api/items/1", "hello");
      Assert.True(result.IsSuccess);
   }

   [Fact]
   public void Created_KindIsCreated()
   {
      var result = Result<string>.Created("/api/items/1", "hello");
      Assert.Equal(SuccessKind.Created, result.Kind);
   }

   [Fact]
   public void Created_SetsLocation()
   {
      var result = Result<string>.Created("/api/items/1", "hello");
      Assert.Equal("/api/items/1", result.Location);
   }

   [Fact]
   public void Created_CarriesValue()
   {
      var result = Result<string>.Created("/api/items/1", "hello");
      Assert.Equal("hello", result.Value);
   }

   // ── Accepted ──────────────────────────────────────────────────────────────

   [Fact]
   public void Accepted_IsSuccess_IsTrue()
   {
      var result = Result<string>.Accepted("hello");
      Assert.True(result.IsSuccess);
   }

   [Fact]
   public void Accepted_KindIsAccepted()
   {
      var result = Result<string>.Accepted("hello");
      Assert.Equal(SuccessKind.Accepted, result.Kind);
   }

   [Fact]
   public void Accepted_WithLocation_SetsLocation()
   {
      var result = Result<string>.Accepted("hello", "/api/items/1/status");
      Assert.Equal("/api/items/1/status", result.Location);
   }

   [Fact]
   public void Accepted_WithoutLocation_LocationIsNull()
   {
      var result = Result<string>.Accepted("hello");
      Assert.Null(result.Location);
   }

   // ── Fail ──────────────────────────────────────────────────────────────────

   [Fact]
   public void Fail_IsSuccess_IsFalse()
   {
      var result = Result<string>.Fail(Error.NotFound());
      Assert.False(result.IsSuccess);
   }

   [Fact]
   public void Fail_CarriesError()
   {
      var error = Error.NotFound("Item not found.");
      var result = Result<string>.Fail(error);

      Assert.Equal(error, result.Error!.Value);
   }

   [Fact]
   public void Fail_ValueIsDefault()
   {
      var result = Result<string>.Fail(Error.NotFound());
      Assert.Null(result.Value);
   }

   // ── Implicit operators ────────────────────────────────────────────────────

   [Fact]
   public void ImplicitFromValue_ProducesOkResult()
   {
      Result<string> result = "hello";

      Assert.True(result.IsSuccess);
      Assert.Equal("hello", result.Value);
      Assert.Equal(SuccessKind.Ok, result.Kind);
   }

   [Fact]
   public void ImplicitFromError_ProducesFailResult()
   {
      Result<string> result = Error.NotFound("Not found.");

      Assert.False(result.IsSuccess);
      Assert.Equal(ErrorType.NotFound, result.Error!.Value.Type);
   }

   // ── Equality ──────────────────────────────────────────────────────────────

   [Fact]
   public void Equality_TwoOkResultsWithSameValue_AreEqual()
   {
      var a = Result<string>.Ok("hello");
      var b = Result<string>.Ok("hello");

      Assert.Equal(a, b);
   }

   [Fact]
   public void Equality_OkAndFail_AreNotEqual()
   {
      var a = Result<string>.Ok("hello");
      var b = Result<string>.Fail(Error.NotFound());

      Assert.NotEqual(a, b);
   }
}