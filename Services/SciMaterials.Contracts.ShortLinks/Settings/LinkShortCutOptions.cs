namespace SciMaterials.Contracts.ShortLinks.Settings;

public record LinkShortCutOptions
(
    int HashStringLength = 5,
    string HashAlgorithm = "SHA512",
    string Encoding = "UTF-32",
    int ConcurrentDbTimeout = 100,
    int ConcurrentDbTryCount = 10
)
{
    public const string SectionName = "LinkShortCutOptions";
    public LinkShortCutOptions() : this(5, "SHA512", "UTF-32", 100, 10) { }
}