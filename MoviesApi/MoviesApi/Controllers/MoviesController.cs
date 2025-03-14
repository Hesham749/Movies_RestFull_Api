using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

    }
}
