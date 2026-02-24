namespace ResultCrafter.Core.Primitives;

/// <summary>
/// Discriminated union representing the outcome of an operation that returns a value.
/// Use <see cref="Ok"/>, <see cref="Created"/>, or <see cref="Accepted"/> for success
/// paths and <see cref="Fail"/> (or an implicit cast from <see cref="Error"/>) for
/// failure paths.
/// </summary>
/// <typeparam name="T">The value type returned on the success path.</typeparam>
public readonly struct Result<T> : IEquatable<Result<T>>
{
   /// <summary>
   /// The success value. Only meaningful when <see cref="IsSuccess"/> is
   /// <see langword="true"/>.
   /// </summary>
   public T? Value { get; }

   /// <summary>
   /// The error produced by this operation. Only meaningful when
   /// <see cref="IsSuccess"/> is <see langword="false"/>.
   /// </summary>
   public Error? Error { get; }

   /// <summary>
   /// The HTTP success variant this result maps to. Always <see cref="SuccessKind.Ok"/>
   /// on failure paths.
   /// </summary>
   public SuccessKind Kind { get; }

   /// <summary>
   /// Resource URI used for HTTP Location headers.
   /// Required for <see cref="SuccessKind.Created"/>; optional poll URL for
   /// <see cref="SuccessKind.Accepted"/>. <see langword="null"/> for all other outcomes.
   /// </summary>
   public string? Location { get; }

   /// <summary>
   /// <see langword="true"/> when the operation succeeded;
   /// <see langword="false"/> when it produced an <see cref="Error"/>.
   /// </summary>
   public bool IsSuccess => Error is null;

   private Result(T? value, Error? error, SuccessKind kind, string? location)
   {
      Value = value;
      Error = error;
      Kind = kind;
      Location = location;
   }

   /// <summary>Creates a 200 OK success result.</summary>
   public static Result<T> Ok(T? value) =>
      new(value, null, SuccessKind.Ok, null);

   /// <summary>
   /// Creates a 201 Created success result.
   /// </summary>
   /// <param name="location">
   /// The URI of the newly created resource. Sent as the HTTP <c>Location</c> header.
   /// </param>
   /// <param name="value">The created resource representation.</param>
   public static Result<T> Created(string location, T value) =>
      new(value, null, SuccessKind.Created, location);

   /// <summary>
   /// Creates a 202 Accepted success result. Use when the request has been queued
   /// or handed off to a background process and completion is not yet confirmed.
   /// </summary>
   /// <param name="value">A representation of the resource as it stands at acceptance time.</param>
   /// <param name="location">
   /// Optional URL the caller can poll to check completion status.
   /// Sent as the HTTP <c>Location</c> header when provided.
   /// </param>
   public static Result<T> Accepted(T value, string? location = null) =>
      new(value, null, SuccessKind.Accepted, location);

   /// <summary>Creates a failure result carrying the given <paramref name="error"/>.</summary>
   public static Result<T> Fail(Error error) =>
      new(default, error, SuccessKind.Ok, null);

   /// <summary>
   /// Implicitly wraps <paramref name="value"/> as an <see cref="Ok"/> result.
   /// Enables <c>return item;</c> in methods returning <c>Result&lt;T&gt;</c>.
   /// </summary>
   public static implicit operator Result<T>(T value) => Ok(value);

   /// <summary>
   /// Implicitly wraps <paramref name="error"/> as a failure result.
   /// Enables <c>return Error.NotFound();</c> in methods returning <c>Result&lt;T&gt;</c>.
   /// </summary>
   public static implicit operator Result<T>(Error error) => Fail(error);

   /// <inheritdoc/>
   public bool Equals(Result<T> other) =>
      EqualityComparer<T?>.Default.Equals(Value, other.Value) &&
      Error == other.Error &&
      Kind == other.Kind &&
      Location == other.Location;

   /// <inheritdoc/>
   public override bool Equals(object? obj) => obj is Result<T> other && Equals(other);

   /// <inheritdoc/>
   public override int GetHashCode() => HashCode.Combine(Value, Error, Kind, Location);

   /// <inheritdoc/>
   public static bool operator ==(Result<T> left, Result<T> right) => left.Equals(right);

   /// <inheritdoc/>
   public static bool operator !=(Result<T> left, Result<T> right) => !left.Equals(right);
}