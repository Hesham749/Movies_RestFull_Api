namespace MoviesApi.DTOs.Movie
{
    public record MovieDetailsDto
    (
        int Id,
        string Title,
        int Year,
        double Rate,
        string StoryLine,
        byte GenreId,
        string GenreName,
        byte[] Poster
    ) : IRecord;
}
