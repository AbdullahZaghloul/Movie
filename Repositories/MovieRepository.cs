using Movies.Repositories.IRepositories;

namespace Movies.Repositories
{
    public class MovieRepository : Repository<Movie>, IMovieRepository
    {
    }
}
