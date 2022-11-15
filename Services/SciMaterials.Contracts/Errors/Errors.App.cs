using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts;

public static partial class Errors
{
    public static class App
    {
            public static readonly Error Unhandled = new("APP000", "Server unhandled error");
    }
}
