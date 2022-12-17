namespace SciMaterials.UI.BWASM.States;

public abstract record CachedState
{
    public DateTime LastUpdated { get; init; }
    public bool IsLoading { get; init; }

    public virtual TimeSpan DefaultCacheDelay { get; init; } = TimeSpan.FromMinutes(3);

    public virtual bool IsNotTimeToUpdateData()
    {
        var isNotTime = LastUpdated != default && DateTime.UtcNow.Subtract(LastUpdated) <= DefaultCacheDelay;
        return isNotTime;
    }
}
