using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts;

public static partial class Errors
{
    /// <summary> Ошибки API (10000-19999). </summary>
    public static partial class Api
    {
        /// <summary> Ошибки сервиса AuthorService (10100-10199). </summary>
        public static class Author
        {
            public static readonly Error NotFound = new("API100", "Author not found");
            public static readonly Error Add = new("API101", "Author add error");
            public static readonly Error Update = new("API102", "Author update error");
            public static readonly Error Delete = new("API0103", "Author delete error");
            public static readonly Error Exist = new("API104", "Author already exist");
        }
    }
}
