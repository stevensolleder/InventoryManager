using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace InventoryManager.Extensions;

public static class ValidationExtensions
{
    public static TValidatedObject ValidateAllProperties<TValidatedObject>(this TValidatedObject validatedObject) where TValidatedObject:notnull
    {
        Validator.ValidateObject(validatedObject, new ValidationContext(validatedObject), true);
        return validatedObject;
    }
    
    public static TValue ValidateProperty<TSource, TValue>(this TSource source, TValue value, [CallerMemberName] string propertyName = "") where TSource:notnull
    {
        Validator.ValidateProperty(value, new ValidationContext(source){MemberName = propertyName});
        return value;
    }
}
