namespace SciMaterials.Contracts.Enums;

public enum ResultCodes
{
    Ok = 0,
    ServerError = 1,
    ApiError = 2,
    ValidationError = 3,
    FileAlreadyExist = 4,
    FormDataFileMissing = 5,
    NotFound = 6,
}

public static class FailureCodes
{
    public static class Authentication
    {
        public const int NoAccessRightsToResource = 401;
    }
}