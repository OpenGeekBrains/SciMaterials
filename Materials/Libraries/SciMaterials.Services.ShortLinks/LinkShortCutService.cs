using System.Security.Cryptography;
using System.Text;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using SciMaterials.Contracts;
using SciMaterials.Contracts.Result;
using SciMaterials.Contracts.ShortLinks;
using SciMaterials.Contracts.ShortLinks.Settings;
using SciMaterials.DAL.Resources.Contexts;

namespace SciMaterials.Services.ShortLinks;

public class LinkShortCutService : ServiceBase, ILinkShortCutService
{
    private readonly SciMaterialsContext _db;
    private readonly LinkGenerator _linkGenerator;
    private readonly HttpContext _httpContext;

    private readonly string _encodingName;
    private readonly int _hashLength;
    private readonly string _hashAlgorithmName;
    private readonly int _concurrentTimeout;
    private readonly int _concurrentTryCount;

    public LinkShortCutService(
        SciMaterialsContext db,
        IOptions<LinkShortCutOptions> options,
        LinkGenerator linkGenerator,
        IHttpContextAccessor httpContextAccessor,
        ILogger<LinkShortCutService> logger) : base(logger)
    {
        _db = db;
        _linkGenerator = linkGenerator;
        _httpContext = httpContextAccessor.HttpContext;

        (
            _hashLength,
            _hashAlgorithmName,
            _encodingName,
            _concurrentTimeout,
            _concurrentTryCount
        ) = options.Value;
    }

    /// <summary>
    /// Добавляет хеш новой ссылки.
    /// Если ссылка уже существует, то возвращает хеш.
    /// </summary>
    /// <param name="sourceAddress"> Адресс ссылки.</param>
    /// <param name="Cancel"> Токен отмены. </param>
    /// <returns> Сокращенный хеш. </returns>
    public async Task<Result<string>> AddAsync(string sourceAddress, CancellationToken Cancel = default)
    {
        var hash = await ComputeHash(sourceAddress, Cancel);
        if (await _db.Links.FirstOrDefaultAsync(l => l.Hash == hash, Cancel) is not { } link)
        {
            link = new() { SourceAddress = sourceAddress, Hash = hash };
            _db.Add(link);
            await _db.SaveChangesAsync(Cancel);
        }

        var shortLink = await GetMinShortLinkAsync(link.Hash, _hashLength, Cancel);
        _Logger.LogInformation("ShortLink {shortLink} for adress {sourceAddress} added", shortLink, sourceAddress);
        return shortLink;
    }

    /// <summary> Возвращает адрес исходной ссылки по хешу. </summary>
    /// <param name="hash"> Сокращенный хеш. </param>
    /// <param name="Cancel"> Токен отмены. </param>
    /// <returns> Адрес исходной ссылки. </returns>
    public async Task<Result<string>> GetAsync(string shortLink, bool isRedirect = false, CancellationToken Cancel = default)
    {
        var hash = shortLink[(shortLink.LastIndexOf('/') + 1)..];
        if (hash.Length < _hashLength)
        {
            _Logger.LogError("Short link {shortLink} hash length must be greater than or equal to {hashLength}", shortLink, _hashLength);
            return Result<String>.Failure(Errors.ShortLink.HashNotFound);

        }

        var link = await _db.Links.SingleOrDefaultAsync(l => l.Hash.StartsWith(hash));
        if (link is not { })
        {
            _Logger.LogError("Short link hash {hash} not found", hash);
            return Result<String>.Failure(Errors.ShortLink.HashNotFound);
        }

        if (isRedirect)
        {
            var registerResult = await RegisterLinkAccess(link.Id, Cancel);
            if (registerResult.IsFaulted)
            {
                return Result<string>.Failure(registerResult);
            }
        }

        return link.SourceAddress;
    }

    public string GetLinkBasePath() =>
        _linkGenerator.GetUriByAction(_httpContext, "GetById", "Links", new { hash = "*" }, _httpContext.Request.Scheme)
            .TrimEnd('*');


    private async Task<Result> RegisterLinkAccess(Guid id, CancellationToken Cancel)
    {
        for (int i = 0; i <= _concurrentTryCount; i++)
        {
            try
            {
                var link = await _db.Links.SingleAsync(l => l.Id == id, Cancel);
                // TODO: обновить свойство в модели на DateTime
                // link.LastAccess = DateTime.Now; 
                link.AccessCount++;
                _db.Update(link);
                await _db.SaveChangesAsync(Cancel);
                return Result.Success();
            }
            catch (DbUpdateConcurrencyException e)
            {
                _Logger.LogWarning(
                    "Error {0} of concurrent write access to the database when updating the record with id: {linkId}",
                    i + 1, id);

                await Task.Delay(_concurrentTimeout, Cancel);
            }
            catch (Exception ex)
            {
                _Logger.LogError(
                    ex,
                    "Register link with id {id} error",
                    id);
                return Result.Failure(Errors.ShortLink.RegisterLinkAccess);
            }
        }
        _Logger.LogError("Link with id {linkId} concurrent update try count expired", id);
        return Result.Failure(Errors.ShortLink.СoncurrentTryCountExpired);
    }


    private async Task<Result<string>> GetMinShortLinkAsync(string hash, int linkLength, CancellationToken Cancel = default)
    {
        var shortLink = hash[..linkLength];

        var linksCount = await _db.Links.CountAsync(item => item.Hash.StartsWith(shortLink), Cancel);
        switch (linksCount)
        {
            case 0:
                return LoggedError<string>(
                    Errors.ShortLink.HashNotFound,
                    "Link with hash {hash} not found",
                    hash);
            case 1:
                var address = _linkGenerator.GetUriByAction(_httpContext, "GetById", "Links", new { hash = shortLink }, _httpContext.Request.Scheme);
                _Logger.LogDebug("Found short link with minimal length: {address}", address);
                return address;
        }

        return await GetMinShortLinkAsync(hash, linkLength++, Cancel);
    }

    private async Task<string> ComputeHash(string text, CancellationToken Cancel = default)
    {
        var encoding = Encoding.GetEncoding(_encodingName);
        var bytes = new MemoryStream(encoding.GetBytes(text));

        var hasher = HashAlgorithm.Create(_hashAlgorithmName)
                     ?? throw new InvalidOperationException(
                         $"Failed to retrieve hashing algorithm {_hashAlgorithmName}");
        var hashBytes = await hasher.ComputeHashAsync(bytes, Cancel).ConfigureAwait(false);
        var hash = Convert.ToBase64String(hashBytes);
        return hash;
    }
}