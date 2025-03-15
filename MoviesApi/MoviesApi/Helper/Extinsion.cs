using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.Helper
{
    public static class Extinsion
    {
        public static CreateGenreDto ToCreateDto(this Genre genre )
        {
            return new CreateGenreDto(genre.Name);
        }
    }
}