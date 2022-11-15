using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts;

public static partial class Errors
{
    /// <summary> Ошибки API (10000-19999). </summary>
    public static partial class Api
    {
        /// <summary> Ошибки сервиса ContentTypeService (10400-10499). </summary>
        public static class ContentType
        {
            public static readonly Error NotFound = new("API400", "ContentType not found");
            public static readonly Error Add = new("API401", "ContentType add error");
            public static readonly Error Update = new("API402", "ContentType update error");
            public static readonly Error Delete = new("API403", "ContentType delete error");
            public static readonly Error Exist = new("API404", "ContentType already exist");
        }
    }
}
