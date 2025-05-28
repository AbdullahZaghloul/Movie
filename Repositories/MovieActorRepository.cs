using Movies.Data;
using Movies.Models;
using Movies.Repositories.IRepositories;

namespace Movies.Repositories
{
    public class MovieActorRepository : Repository<ActorMovie>, IMoveActorRepository
    {
        private readonly ApplicationDbContext _context;
        public MovieActorRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
