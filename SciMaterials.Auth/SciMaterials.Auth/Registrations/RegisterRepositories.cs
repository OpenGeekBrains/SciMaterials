using SciMaterials.Auth.Data.AuthRepository;

namespace SciMaterials.Auth.Registrations;

public static class RegisterRepositories
{
    public static IServiceCollection RegisterAuthRepositories(this IServiceCollection services)
    {
        return services
            .AddTransient<IUsersRepository, UsersRepository>()
            .AddTransient<IUserRolesRepository, UserRolesRepository>();
    }
}