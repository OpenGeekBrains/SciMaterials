using System.Collections.Immutable;

namespace SciMaterials.UI.BWASM.States.Authors;

public static class AuthorsActions
{
    public record struct LoadAuthorsAction;
    public record struct LoadAuthorsResultAction(ImmutableArray<AuthorState> Authors);

    public static LoadAuthorsAction LoadAuthors() => new();
    public static LoadAuthorsResultAction LoadAuthorsResult(ImmutableArray<AuthorState> authors) => new(authors);
}