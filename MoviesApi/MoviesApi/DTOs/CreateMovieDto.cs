namespace MoviesApi.DTOs
{
    public class CreateMovieDto
    {
        [Required, MaxLength(250)]
        public string Title { get; set; }
        public int Year { get; set; }
        public double Rate { get; set; }
        [Required, MaxLength(2500)]
        public string StoryLine { get; set; }
        [Required]
        public IFormFile Poster { get; set; }
        public byte GenreId { get; set; }
    }
}
