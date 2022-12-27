using SciMaterials.Contracts.Result;

// ReSharper disable once CheckNamespace
namespace SciMaterials.Contracts;

public static partial class Errors
{
    /// <summary> Ошибки API (10000-19999). </summary>
    public static partial class Api
    {
        /// <summary> Ошибки сервиса ResourcesUsersService (RESU000-RESU099). </summary>
        public static class ResourcesUsers
        {
            public static readonly Error NotFound = new("RESU000", "Resources User not found");
            public static readonly Error Add = new("RESU001", "Resources User register error");
            public static readonly Error Update = new("RESU002", "Resources User update error");
            public static readonly Error Delete = new("RESU003", "Resources User delete error");
            public static readonly Error Exist = new("RESU004", "Resources User already exist");
        }
    }
}
