using ApiPeliculas.Feats.Categories.Repository;
using ApiPeliculas.Feats.Movies.Repository;

namespace ApiPeliculas.Extensions;

public static class AppServiceExtensions
{
    public static void AddAppServices(this IServiceCollection services)
    {
        services.AddTransient<ICategoryRepository, CategoryRepository>();
        services.AddTransient<IMovieRepository, MovieRepository>();
    }
}
