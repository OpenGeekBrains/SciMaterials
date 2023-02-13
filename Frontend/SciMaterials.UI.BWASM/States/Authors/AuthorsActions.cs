using System.Collections.Immutable;

namespace SciMaterials.UI.BWASM.States.Authors;

public static class AuthorsActions
{
    public record struct LoadAuthorsAction;
    public record struct ForceReloadAuthorsAction;
    public record struct LoadAuthorsStartAction;
    public record struct LoadAuthorsResultAction(ImmutableArray<AuthorState> Authors);

    public static LoadAuthorsAction LoadAuthors() => new();
    public static ForceReloadAuthorsAction ForceReloadAuthors() => new();
    public static LoadAuthorsStartAction LoadAuthorsStart() => new();
    public static LoadAuthorsResultAction LoadAuthorsResult(ImmutableArray<AuthorState> authors) => new(authors);
}