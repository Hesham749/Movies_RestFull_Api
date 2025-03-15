namespace MoviesApi.DTOs.Genre
{
    public record CreateGenreDto
    (
        [Required, StringLength(100 , MinimumLength =3)]
         string Name
    ) : IRecord;
}
