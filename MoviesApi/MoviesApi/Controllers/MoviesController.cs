using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace MoviesApi.Controllers
{
    [Route("api/Movies")]
    [ApiController]
    [Produces("application/json")]
    public class MoviesController : ControllerBase
    {
        private readonly IUnitOfWorks _unitOfWorks;

        public MoviesController(IUnitOfWorks unitOfWorks)
        {
            _unitOfWorks = unitOfWorks;
        }

        private long _maxAllowedPosterSize = 1024 * 1024;
        private List<string> _AllowedExtisions = [".jpg", ".png"];


        [HttpGet]
        [ProducesResponseType(typeof(List<MovieDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllAsync()
        {
            var movies = await _unitOfWorks.Movies.GetAllAsync();
            if (!movies.Any()) return NotFound($"No movies where found!");
            return Ok(movies.ToList()
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
            )));

        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(MovieDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var movie = await _unitOfWorks.Movies.GetByIdAsync(id);
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
            var genre = await _unitOfWorks.Genres.GetByIdAsync(id);
            if (genre is null) return BadRequest($"No genre with id {id}");
            var movies = await _unitOfWorks.Movies.GetAllAsync(m => m.GenreId == id);
            if (!movies.Any()) return NotFound($"No Movies in {genre.Name} genre!");
            return Ok(
            movies.ToList()
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
            ));
        }

        [HttpPost]
        [ProducesResponseType(typeof(Movie), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAsync([FromForm] MovieDto dto)
        {
            if (dto.Poster == null) return BadRequest($"Poster is required!");

            if (!_AllowedExtisions.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                return BadRequest($"Image Must be jpg or png only!");

            if (dto.Poster.Length > _maxAllowedPosterSize)
                return BadRequest($"Max allowed image size is 1MB!");

            var isValidGenre = await _unitOfWorks.Movies.AnyAsync(m => m.GenreId == dto.GenreId);
            if (!isValidGenre) return BadRequest($"Invalid genre ID!");

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
            await _unitOfWorks.Movies.AddAsync(movie);
            _unitOfWorks.Save();
            return Ok(movie);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromForm] MovieDto dto)
        {

            var isValidGenre = await _unitOfWorks.Genres.AnyAsync(g => g.Id == dto.GenreId);
            if (!isValidGenre) return BadRequest($"Invalid genre ID!");

            var movie = await _unitOfWorks.Movies.GetByIdAsync(id);
            if (movie is null) return NotFound($"No movie was found with id {id}!");
            if (dto.Poster is not null)
            {
                if (!_AllowedExtisions.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                    return BadRequest($"Image Must be jpg or png only!");

                if (dto.Poster.Length > _maxAllowedPosterSize)
                    return BadRequest($"Max allowed image size is 1MB!");

                var dataStream = new MemoryStream();
                dto.Poster.CopyTo(dataStream);
                movie.Poster = dataStream.ToArray();
            }

            movie.Title = dto.Title;
            movie.Rate = dto.Rate;
            movie.StoryLine = dto.StoryLine;
            movie.Year = dto.Year;

            _unitOfWorks.Save();

            return Ok(movie);
        }


        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var movie = await _unitOfWorks.Movies.GetByIdAsync(id);
            if (movie is null) return BadRequest($"No movie with id {id}!");
            _unitOfWorks.Movies.Delete(movie);
            _unitOfWorks.Save();
            return Ok("Movie deleted successfully!");
        }

    }
}
