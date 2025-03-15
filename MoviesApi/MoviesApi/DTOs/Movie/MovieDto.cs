namespace MoviesApi.DTOs.Movie
{
    public record MovieDto(
        [Required, MaxLength(250)]
        string Title,
        int Year,
        double Rate,
        [Required, MaxLength(2500)]
        string StoryLine,
        IFormFile Poster,
        byte GenreId
    ) : IRecord;
}
