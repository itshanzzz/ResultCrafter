using FluentValidation;
using ResultCrafter.Core.Primitives;

namespace ResultCrafter.FluentValidation;

public static class ValidatorExtensions
{
   /// <summary>
   ///    Validates <paramref name="instance" /> and returns <see langword="null" /> on success,
   ///    or an <see cref="Error.BadRequest(Dictionary{string,string[]},string?)" /> with
   ///    field-level errors keyed by FluentValidation's property name on failure.
   /// </summary>
   /// <remarks>
   ///    Property names are used as-is from FluentValidation (e.g. <c>"FirstName"</c>,
   ///    <c>"Address.Street"</c>). Configure FluentValidation's
   ///    <c>ValidatorOptions.Global.PropertyNameResolver</c> in your composition root if
   ///    you need a different casing convention (e.g. camelCase).
   /// </remarks>
   public static async Task<Error?> ValidateToResultAsync<T>(this IValidator<T> validator,
      T instance,
      CancellationToken ct = default)
   {
      var result = await validator.ValidateAsync(instance, ct);

      if (result.IsValid)
      {
         return null;
      }

      var errors = result.Errors
                         .GroupBy(e => e.PropertyName, StringComparer.OrdinalIgnoreCase)
                         .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.ErrorMessage)
                                  .ToArray(),
                            StringComparer.OrdinalIgnoreCase);

      return Error.BadRequest(errors);
   }
}