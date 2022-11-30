using SciMaterials.Contracts.Result;

// ReSharper disable once CheckNamespace
namespace SciMaterials.Contracts;

public static partial class Errors
{
    public static class App
    {
        public static readonly Error Unhandled = new("APP000", "Server unhandled error");
        public static readonly Error ParseFailure = new("APP001", "Fail to parse response");
        public static readonly Error OperationCanceled = new("APP002", "Operation cancelled");
        public static readonly Error ClientUnhandled = new("APP003", "Client unhandled error");
    }
}
