namespace MoviesApi.DTOs
{
    public class CreateGenreDto
    {
        [Required, StringLength(100 , MinimumLength =3)]
        public string Name { get; set; }
    }
}
