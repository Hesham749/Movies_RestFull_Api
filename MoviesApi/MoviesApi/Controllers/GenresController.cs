using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoviesApi.Interfaces;

namespace MoviesApi.Controllers
{
    [Route("api/Genres")]
    [ApiController]
    [Produces("application/json")]
    public class GenresController : ControllerBase
    {
        private readonly IUnitOfWorks _unitOfWork;

        public GenresController(IUnitOfWorks unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<Genre>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllAsync()
        {
            var Genres = await _unitOfWork.Genres.GetAllAsync();
            return Genres.Any() ? Ok(Genres) : NotFound("No Genres Was Found");
        }

        [HttpPost]
        [ProducesResponseType(typeof(Genre), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAsync([FromBody] CreateGenreDto dto)
        {
            var genre = new Genre { Name = dto.Name };
            await _unitOfWork.Genres.AddAsync(genre);
            _unitOfWork.Save();
            return Ok(genre);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Genre), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAsync([FromRoute] int id, [FromBody] CreateGenreDto dto)
        {
            var genre = await _unitOfWork.Genres.GetByIdAsync(id);
            if (genre is null)
                return NotFound($"No genre with id {id} was found");
            genre.Name = dto.Name;
            _unitOfWork.Save();
            return Ok(genre);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {
            //var genre = await _context.Genres.FirstOrDefaultAsync(g => g.Id == id);
            var genre = await _unitOfWork.Genres.GetByIdAsync(id);
            if (genre is null)
                return NotFound($"No genre with id {id} was found");
            _unitOfWork.Genres.Delete(genre);
            _unitOfWork.Save();
            return Ok("Genre Deleted Successfully");
        }
    }
}
