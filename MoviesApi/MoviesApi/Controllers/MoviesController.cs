using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoviesApi.DTOs.Movie;
using MoviesApi.Models;

namespace MoviesApi.Controllers
{
    [Route("api/Movies")]
    [ApiController]
    [Produces("application/json")]
    public class MoviesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private long _maxAllowedPosterSize = 1024 * 1024;
        private List<string> _AllowedExtisions = [".jpg", ".png"];
        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<MovieDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllAsync()
        {
            var movies = await _context.Movies
                .OrderByDescending(m => m.Rate)
                .Include(m => m.Genre)
                .Select(m => new MovieDetailsDto
                (
                     m.Id,
                     m.Title,
                    m.Year,
                     m.Rate,
                     m.StoryLine,
                     m.GenreId,
                     m.Genre.Name,
                     m.Poster
                ))
                .AsNoTracking()
                .ToListAsync();
            return movies.Count == 0 ? NotFound($"No movies were found") : Ok(movies);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(MovieDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var movie = await _context.Movies.Include(m => m.Genre)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.Id == id);
            if (movie is null)
                return NotFound($"No movie with id {id}");
            var dto = new MovieDetailsDto(
                id,
                movie.Title,
                movie.Year,
                movie.Rate,
                movie.StoryLine,
                movie.GenreId,
                movie.Genre.Name,
                movie.Poster
                );
            return Ok(dto);
        }

        [HttpGet("GetByGenreId/{id}")]
        [ProducesResponseType(typeof(List<MovieDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByGenreIdAsync(int id)
        {
            var genre = await _context.Genres.SingleOrDefaultAsync(m => m.Id == id);
            if (genre is null) return BadRequest($"No genre with id {id}");
            var movies = await _context.Movies
            .Where(m => m.GenreId == id)
            .OrderBy(m => m.Rate)
            .Include(m => m.Genre)
            .Select(m => new MovieDetailsDto(
                m.Id,
                m.Title,
                m.Year,
                m.Rate,
                m.StoryLine,
                m.GenreId,
                m.Genre.Name,
                m.Poster
                )
            )
            .AsNoTracking()
            .ToListAsync();

            return movies.Count == 0 ? NotFound($"No Movies in {genre.Name} genre!") : Ok(movies);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Movie), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAsync([FromForm] CreateMovieDto dto)
        {
            if (!_AllowedExtisions.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                return BadRequest($"Image Must be jpg or png only!");

            if (dto.Poster.Length > _maxAllowedPosterSize)
                return BadRequest($"Max allowed image size is 1MB!");

            using var dataStream = new MemoryStream();
            await dto.Poster.CopyToAsync(dataStream);
            var movie = new Movie
            {
                GenreId = dto.GenreId,
                Title = dto.Title,
                Poster = dataStream.ToArray(),
                Rate = dto.Rate,
                StoryLine = dto.StoryLine,
                Year = dto.Year,
            };
            await _context.AddAsync(movie);
            await _context.SaveChangesAsync();
            return Ok(movie);
        }


        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie is null) return BadRequest($"No movie with id {id}!");
            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
            return Ok("Movie deleted successfully!");
        }

    }
}
