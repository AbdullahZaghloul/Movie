using Movies.Models;
using Movies.Repositories.IRepositories;

namespace Movies.Repositories
{
    public class CinemaRepository:Repository<Cinema>,ICinemaRepository
    {
    }
}
