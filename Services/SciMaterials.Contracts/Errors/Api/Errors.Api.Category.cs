using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts;

public static partial class Errors
{
    /// <summary> Ошибки API (10000-19999). </summary>
    public static partial class Api
    {
        /// <summary> Ошибки сервиса CategoryService (10200-10299). </summary>
        public static class Category
        {
            public static readonly Error NotFound = new("API200", "Category not found");
            public static readonly Error Add = new("API201", "Category add error");
            public static readonly Error Update = new("API202", "Category update error");
            public static readonly Error Delete = new("API203", "Category delete error");
            public static readonly Error Exist = new("API204", "Category already exist");
        }
    }
}
