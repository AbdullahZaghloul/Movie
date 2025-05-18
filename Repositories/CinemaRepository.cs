using Movies.Data;
using Movies.Models;
using Movies.Repositories.IRepositories;

namespace Movies.Repositories
{
    public class CinemaRepository : Repository<Cinema>, ICinemaRepository
    {
        private readonly ApplicationDbContext _context;
        public CinemaRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
