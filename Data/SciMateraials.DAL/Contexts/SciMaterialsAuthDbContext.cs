using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SciMaterials.DAL.Contexts;

public class SciMaterialsAuthDbContext : IdentityDbContext<IdentityUser>
{
    public SciMaterialsAuthDbContext(DbContextOptions<SciMaterialsAuthDbContext> options) : base(options)
    {
        //Database.EnsureCreated();
    }
}