using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts;

public static partial class Errors
{
    /// <summary> Ошибки API (10000-19999). </summary>
    public static partial class Api
    {
        /// <summary> Ошибки сервиса CommentService (10300-10399). </summary>
        public static class Comment
        {
            public static readonly Error NotFound = new("API300", "Comment not found");
            public static readonly Error Add = new("API301", "Comment add error");
            public static readonly Error Update = new("API302", "Comment update error");
            public static readonly Error Delete = new("API303", "Comment delete error");
            public static readonly Error Exist = new("API304", "Comment already exist");
        }
    }
}