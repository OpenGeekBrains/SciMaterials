using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts;

public static partial class Errors
{
    /// <summary> Ошибки API (10000-19999). </summary>
    public static partial class Api
    {
        /// <summary> Ошибки сервиса UrlService (10600-10699). </summary>
        public static class Url
        {
            public static readonly Error NotFound = new("API600", "Url not found");
            public static readonly Error Add = new("API601", "Url add error");
            public static readonly Error Update = new("API602", "Url update error");
            public static readonly Error Delete = new("API603", "Url delete error");
            public static readonly Error Exist = new("API604", "Url already exist");
        }
    }
}
