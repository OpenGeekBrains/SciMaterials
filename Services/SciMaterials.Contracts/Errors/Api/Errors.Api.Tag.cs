using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts;

public static partial class Errors
{
    /// <summary> Ошибки API (10000-19999). </summary>
    public static partial class Api
    {
        /// <summary> Ошибки сервиса TagService (10500-10599). </summary>
        public static class Tag
        {
            public static readonly Error NotFound = new("API500", "Tag not found");
            public static readonly Error Add = new("API501", "Tag add error");
            public static readonly Error Update = new("API502", "Tag update error");
            public static readonly Error Delete = new("API503", "Tag delete error");
            public static readonly Error Exist = new("API504", "Tag already exist");
        }
    }
}
