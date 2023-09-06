using ApiPeliculas.Feats.Movies.DTOs;
using ApiPeliculas.Feats.Movies.Repository;
using ApiPeliculas.Shared;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace ApiPeliculas.Feats.Movies.Controllers;

[ApiController]
[Route("api/peliculas")]
public class MoviesController : ControllerBase
{
    private readonly IMovieRepository movieRepository;

    public MoviesController(IMovieRepository movieRepository)
    {
        this.movieRepository = movieRepository;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMovies()
    {
        ICollection<Movie> moviesDb =
            await movieRepository.GetMovies();

        if (moviesDb.Count == 0)
        {
            return NotFound("No hay películas que mostrar");
        }

        ICollection<MovieDto> categories =
            moviesDb.Adapt<ICollection<MovieDto>>();

        return Ok(categories);
    }

    [HttpGet("{id:Guid}", Name = "pelicula")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCategory(Guid id)
    {
        Movie? movieDb =
            await movieRepository.GetMovieById(id);

        if (movieDb is null)
        {
            return NotFound(id);
        }

        MovieDto movie = movieDb.Adapt<MovieDto>();

        return Ok(movie);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateMovie([FromBody] MovieDto movie)
    {
        if (!ModelState.IsValid || movie is null)
        {
            ModelState.AddModelError(string.Empty, "Revise los campos");
            return BadRequest(ModelState);
        }

        if (movieRepository.MovieExists(movie.Name))
        {
            ModelState.AddModelError(string.Empty, "La película ya existe");
            return StatusCode(404, ModelState);
        }

        Movie movieToBd = movie.Adapt<Movie>();
        bool result = await movieRepository.CreateMovie(movieToBd);
        if (!result)
        {
            ModelState.AddModelError(string.Empty, "Ha habido un error y no se pudo guardar el registro por favor consulte con el adminsitrador del sistema");
            return StatusCode(500, ModelState);
        }

        return CreatedAtRoute("pelicula", new { id = movieToBd.Id }, movieToBd);
    }

    [HttpPatch("{id:Guid}", Name = "actualizarPelicula")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PatchMovie(Guid id, [FromBody] MovieDto movie)
    {
        if (!ModelState.IsValid || movie is null || id != movie.Id)
        {
            return BadRequest(ModelState);
        }

        Movie movieTobd = movie.Adapt<Movie>();
        bool result = await movieRepository.UpdateMovie(movieTobd);

        if (!result)
        {
            ModelState.AddModelError(string.Empty, "Ha habido un error y no se pudo actualizar el registro por favor consulte con el adminsitrador del sistema");
            return StatusCode(500, ModelState);
        }

        return NoContent();
    }

    [HttpDelete("{id:Guid}", Name = "borrarPelicula")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteMovie(Guid id)
    {
        if (!movieRepository.MovieExists(id))
        {
            return NotFound(id);
        }

        Movie? movieToDelete = await movieRepository.GetMovieById(id);
        bool result = false;
        if (movieToDelete is not null)
        {
            result = await movieRepository.DeleteMovie(movieToDelete);
        }

        if (!result)
        {
            ModelState.AddModelError(string.Empty, "Ha habido un error y no se pudo eliminar el registro por favor consulte con el adminsitrador del sistema");
            return StatusCode(500, ModelState);
        }

        return NoContent();
    }

    [HttpGet("ObtenerPeliculasEnCategoria/{categoriaId:Guid}")]
    public async Task<IActionResult> GetMoviesinCategories(Guid categoriaId)
    {
        ICollection<Movie> moviesDb =
            await movieRepository.GetMoviesInCategories(categoriaId);

        if (moviesDb.Count == 0)
        {
            return NotFound("No hay películas que mostrar");
        }

        ICollection<MovieDto> categories =
            moviesDb.Adapt<ICollection<MovieDto>>();

        return Ok(categories);
    }
}
