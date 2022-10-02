using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SciMaterials.Auth.Data.Models;

namespace SciMaterials.Auth.Data.Context;

public class AuthDbContext : IdentityDbContext<CustomIdentityUser>
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
}