namespace MoviesApi.Helper
{
    public static class Extinsion
    {
        public static CreateGenreDto ToCreateDto(this Genre genre)
        {
            return new CreateGenreDto(genre.Name);
        }

        public static MovieDetailsDto ToMoviesDto(this Movie movie)
        {
            return new MovieDetailsDto(
                    movie.Id,
                    movie.Title,
                    movie.Year,
                    movie.Rate,
                    movie.StoryLine,
                    movie.GenreId,
                    movie.Genre.Name,
                    movie.Poster
                );
        }
        public static Movie ToMovie(this MovieDto dto)
        {
            using var dataStream = new MemoryStream();
            dto.Poster.CopyTo(dataStream);
            return new Movie
            {
                GenreId = dto.GenreId,
                Title = dto.Title,
                Poster = dataStream.ToArray(),
                Rate = dto.Rate,
                StoryLine = dto.StoryLine,
                Year = dto.Year,
            };
        }
    }
}