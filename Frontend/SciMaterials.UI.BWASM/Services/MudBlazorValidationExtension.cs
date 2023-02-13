using FluentValidation;

namespace SciMaterials.UI.BWASM.Services;

public static class MudBlazorValidationExtension
{
    public static Func<object, string, Task<IEnumerable<string>>> ValidateValue<T>(this IValidator<T> self) => async (model, propertyName) =>
    {
        var result = await self.ValidateAsync(ValidationContext<T>.CreateWithOptions((T)model, x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
        {
            return Array.Empty<string>();
        }
        return result.Errors.Select(e => e.ErrorMessage);
    };
}