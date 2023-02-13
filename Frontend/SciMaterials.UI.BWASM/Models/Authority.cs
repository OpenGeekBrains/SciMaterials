using System.Xml.Linq;

namespace SciMaterials.UI.BWASM.Models;

public class Authority
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; set; } = null!;

    public static Authority Create(string name) =>
        new(){Name = name};

    public static Authority Create(Authority origin) =>
        new() { Id = origin.Id, Name = origin.Name };
}