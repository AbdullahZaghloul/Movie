using Movies.Data;
using Movies.Models;
using Movies.Repositories.IRepositories;

namespace Movies.Repositories
{
    public class ActorRepository : Repository<Actor>, IActorRepository
    {
        private readonly ApplicationDbContext _context;
        public ActorRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

       
    }
}
