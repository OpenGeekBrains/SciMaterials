namespace SciMaterials.DAL.Contracts.Configuration;

public sealed class DatabaseSettings
{
    public string? Provider { get; init; }
    public bool RemoveAtStart { get; init; }
    public bool UseDataSeeder { get; init; }

    public string GetProviderName()
    {
        if (Provider is not { } provider)
            throw new InvalidOperationException("Не задан провайдер");

        var dot_index = provider.IndexOf('.');
        if (dot_index == -1)
            throw new ArgumentNullException(string.Empty, "Provider not set in configuration file");

        return provider[..dot_index];
    }
}