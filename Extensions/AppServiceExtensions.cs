using ApiPeliculas.Data.Repository;
using ApiPeliculas.Data.Repository.Interfaces;

namespace ApiPeliculas.Extensions;

public static class AppServiceExtensions
{
    public static void AddAppServices(this IServiceCollection services)
    {
        services.AddTransient<ICategoryRepository, CategoryRepository>();
    }
}
