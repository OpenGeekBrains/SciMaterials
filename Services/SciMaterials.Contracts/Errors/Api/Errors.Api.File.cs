using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts;

public static partial class Errors
{
    /// <summary> Ошибки API (10000-19999). </summary>
    public static partial class Api
    {
        /// <summary> Ошибки сервиса FileService (10000-10099). </summary>
        public static class File
        {
            public static readonly Error NotFound = new("API000", "File not found");
            public static readonly Error Add = new("API001", "File add to database error");
            public static readonly Error Update = new("API002", "File update error");
            public static readonly Error Delete = new("API003", "File delete error");
            public static readonly Error Download = new("API004", "File download error");
            public static readonly Error Exist = new("API005", "File already exist");
            public static readonly Error ContentTypeNotFound = new("API006", "File content type not found");
            public static readonly Error CategoriesAreNotSpecified = new("API007", "File categories are not specified");
            public static readonly Error CategoryNotFound = new("API008", "File category not found");
            public static readonly Error AuthorNotFound = new("API009", "File author not found");
            public static readonly Error StoreWrite = new("API010", "File write to store error");
            public static readonly Error MissingMetadata = new("API011", "Metadata is missing");
            public static readonly Error MissingSection = new("API012", "Form-data sections does not contains files");
        }
    }
}
