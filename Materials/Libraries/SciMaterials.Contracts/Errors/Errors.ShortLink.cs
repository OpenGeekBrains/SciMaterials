using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts;

public static partial class Errors
{
    public static class ShortLink
    {
        public static readonly Error HashNotFound = new("SRV000", "Link hash not found");
        public static readonly Error RegisterLinkAccess = new("SRV001", "Register link access error");
        public static readonly Error СoncurrentTryCountExpired = new("SRV002", "Link concurrent update try count expired");
    }
}
