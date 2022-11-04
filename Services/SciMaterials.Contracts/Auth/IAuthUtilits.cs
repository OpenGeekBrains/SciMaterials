using Microsoft.AspNetCore.Identity;

namespace SciMaterials.Contracts.Auth;

public interface IAuthUtilits : IAuthUtils<IdentityUser> { }
