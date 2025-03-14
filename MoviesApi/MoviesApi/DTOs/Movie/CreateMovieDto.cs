namespace MoviesApi.DTOs.Movie
{
    public record CreateMovieDto(
        [Required, MaxLength(250)]
        string Title,
        int Year,
        double Rate,
        [Required, MaxLength(2500)]
        string StoryLine,
        [Required]
        IFormFile Poster,
        byte GenreId
    );
}
