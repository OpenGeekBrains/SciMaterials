namespace SciMaterials.UI.BWASM.Models;

public class AuthorityGroup
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; set; } = null!;
    public List<Authority> Authorities { get; set; } = new();

    public static AuthorityGroup Create(string name, params Authority[] authorities) =>
        new() { Name = name, Authorities = authorities.ToList()};

    public static AuthorityGroup Create(AuthorityGroup origin) =>
        new () { Id = origin.Id, Name = origin.Name, Authorities = origin.Authorities.Select(x=>Authority.Create(x)).ToList() };
}