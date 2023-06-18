using Microsoft.EntityFrameworkCore;

using SciMaterials.DAL.Resources.TestData;

namespace SciMaterials.UrlsService.Api.Data;

internal class UrlsStorageContext : DbContext
{
	public UrlsStorageContext(DbContextOptions<UrlsStorageContext> options) : base(options)
	{
	}

	public virtual DbSet<UrlEntity> Links { get; set; } = null!;

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);
		modelBuilder.Entity<UrlEntity>(link =>
		{
			link.HasKey(e => e.ShortenedRouteSegment);
			link.Property(e => e.AccessCount).IsConcurrencyToken();
			link.Property(e => e.LastAccess).IsConcurrencyToken();
			link.Property(e => e.RowVersion).IsRowVersion();
		});
	}

	public UrlEntity Add(string sourceAddress)
	{
		var shortenedRouteSegment = Guid.NewGuid().GetHashCode().ToString("X");

		UrlEntity entity = new() { ShortenedRouteSegment = shortenedRouteSegment, SourceAddress = sourceAddress };
		Links.Add(entity);

		return entity;
	}

	public static void UpdateLastAccess(UrlEntity entity)
	{
		entity.AccessCount++;
		entity.LastAccess = DateTime.UtcNow;
	}

	public async Task Initialize(ILogger<UrlsStorageContext> logger, CancellationToken cancellationToken)
	{
		logger.LogInformation("Инициализация БД тестовыми данными");

		await using var transaction = await Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);

		if (!await Links.AnyAsync(cancellationToken))
		{
			try
			{
				var links = AssemblyResources.Links;

                foreach (var link in links)
                {
					Add(link.SourceAddress).SetValues(u =>
					{
						u.AccessCount = link.AccessCount;
						u.LastAccess = link.LastAccess;
						u.RowVersion = link.RowVersion;
					});
                }
                await SaveChangesAsync(cancellationToken);
			}
			catch (Exception e)
			{
				logger.LogError(e, "Error loading data Links");
				throw;
			}
		}

		await transaction.CommitAsync(cancellationToken);
	}
}

public class UrlEntity
{
	public required string ShortenedRouteSegment { get; init; }

	public string SourceAddress { get; set; } = string.Empty;
	
	public int AccessCount { get; set; }
	public DateTime? LastAccess { get; set; }
	public byte[] RowVersion { get; set; } = null!;
}

static class Extensions
{
	public static void SetValues<T>(this T self, Action<T> setter)
	{
		setter(self);
	}
}