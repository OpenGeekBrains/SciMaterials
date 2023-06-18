using Microsoft.EntityFrameworkCore;

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

	public async ValueTask<UrlEntity> Add(string sourceAddress)
	{
		var shortenedRouteSegment = Guid.NewGuid().GetHashCode().ToString("X");

		UrlEntity entity = new() { ShortenedRouteSegment = shortenedRouteSegment, SourceAddress = sourceAddress };
		await Links.AddAsync(entity);
		await SaveChangesAsync();

		return entity;
	}

	public async ValueTask UpdateLastAccess(UrlEntity entity)
	{
		entity.AccessCount++;
		entity.LastAccess = DateTime.UtcNow;
		await SaveChangesAsync();
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

