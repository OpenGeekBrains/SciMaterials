using System.Collections.Immutable;

namespace SciMaterials.UI.BWASM.Extensions;

public static class ImmutableArrayExtensions
{
    public static bool ReplaceOne<T>(this ImmutableArray<T> source, Predicate<T> selector, Func<T, T> replacement, out ImmutableArray<T> result)
    {
        for (int i = 0; i < source.Length; i++)
        {
            T item = source[i];
            if (!selector(item)) continue;

            result = source.SetItem(i, replacement(item));
            return true;
        }

        result = source;
        return false;
    }
}