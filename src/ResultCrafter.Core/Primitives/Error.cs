namespace ResultCrafter.Core.Primitives;

/// <summary>
/// An immutable, typed error value. Carry one through a <see cref="Result{T}"/> or
/// <see cref="Result"/> rather than throwing exceptions for expected failure paths.
/// </summary>
/// <remarks>
/// All factory methods enforce the minimum data contract for each error type.
/// <see cref="BadRequest(Dictionary{string,string[]},string?)"/> requires a field-errors
/// dictionary; <see cref="BadRequest(string)"/> requires a prose detail message.
/// </remarks>
public readonly struct Error : IEquatable<Error>
{
   /// <summary>The category of this error. Determines the HTTP status code and ProblemDetails title.</summary>
   public ErrorType Type { get; }

   /// <summary>
   /// A human-readable description of the error. Falls back to a catalog default when
   /// <see langword="null"/>. Always populated for <see cref="BadRequest(string)"/>.
   /// </summary>
   public string? Detail { get; }

   /// <summary>
   /// Field-level error messages, keyed by field name. Only populated for errors
   /// constructed via <see cref="BadRequest(Dictionary{string,string[]},string?)"/>.
   /// <see langword="null"/> otherwise.
   /// </summary>
   public IReadOnlyDictionary<string, string[]>? Errors { get; }

   /// <summary>
   /// <see langword="true"/> when this error carries a field-level <see cref="Errors"/>
   /// dictionary (i.e. was constructed via
   /// <see cref="BadRequest(Dictionary{string,string[]},string?)"/>).
   /// The HTTP response will include an <c>errors</c> extension containing the dictionary.
   /// </summary>
   public bool IsValidation => Errors is not null;

   private Error(ErrorType type, string? detail, Dictionary<string, string[]>? errors)
   {
      Type = type;
      Detail = detail;
      Errors = errors;
   }

   // ── Simple errors ─────────────────────────────────────────────────────────

   /// <summary>Creates a <see cref="ErrorType.NotFound"/> error (HTTP 404).</summary>
   public static Error NotFound(string? detail = null) => new(ErrorType.NotFound, detail, null);

   /// <summary>Creates an <see cref="ErrorType.Unauthorized"/> error (HTTP 401).</summary>
   public static Error Unauthorized(string? detail = null) => new(ErrorType.Unauthorized, detail, null);

   /// <summary>Creates a <see cref="ErrorType.Forbidden"/> error (HTTP 403).</summary>
   public static Error Forbidden(string? detail = null) => new(ErrorType.Forbidden, detail, null);

   /// <summary>Creates a <see cref="ErrorType.Conflict"/> error (HTTP 409).</summary>
   public static Error Conflict(string? detail = null) => new(ErrorType.Conflict, detail, null);

   /// <summary>
   /// Creates a <see cref="ErrorType.ConcurrencyConflict"/> error (HTTP 409).
   /// Use this instead of <see cref="Conflict"/> when the conflict is specifically
   /// an optimistic-concurrency token mismatch.
   /// </summary>
   public static Error ConcurrencyConflict(string? detail = null) => new(ErrorType.ConcurrencyConflict, detail, null);

   // ── BadRequest ────────────────────────────────────────────────────────────

   /// <summary>
   /// Creates a <see cref="ErrorType.BadRequest"/> error (HTTP 400) with a prose reason.
   /// </summary>
   /// <param name="detail">Required human-readable reason for the rejection.</param>
   public static Error BadRequest(string detail) => new(ErrorType.BadRequest, detail, null);

   /// <summary>
   /// Creates a <see cref="ErrorType.BadRequest"/> error (HTTP 400) with field-level
   /// validation errors. The response includes an <c>errors</c> extension containing the
   /// field dictionary — matching the shape consumers already expect from ASP.NET Core
   /// model validation responses.
   /// </summary>
   /// <param name="errors">
   /// Field-level errors keyed by field name (e.g. <c>"email"</c>),
   /// each with one or more error messages.
   /// </param>
   /// <param name="detail">Optional summary. Defaults to a catalog value when <see langword="null"/>.</param>
   public static Error BadRequest(Dictionary<string, string[]> errors, string? detail = null) =>
      new(ErrorType.BadRequest, detail, errors);

   // ── Equality ──────────────────────────────────────────────────────────────

   /// <inheritdoc/>
   public bool Equals(Error other) =>
      Type == other.Type &&
      Detail == other.Detail &&
      Equals(Errors, other.Errors);

   /// <inheritdoc/>
   public override bool Equals(object? obj) => obj is Error other && Equals(other);

   /// <inheritdoc/>
   public override int GetHashCode() => HashCode.Combine(Type, Detail);

   /// <inheritdoc/>
   public static bool operator ==(Error left, Error right) => left.Equals(right);

   /// <inheritdoc/>
   public static bool operator !=(Error left, Error right) => !left.Equals(right);

   /// <inheritdoc/>
   public override string ToString() => Detail is not null ? $"{Type}: {Detail}" : Type.ToString();
}