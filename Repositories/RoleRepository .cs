using Microsoft.AspNetCore.Identity;
using Movies.Data;
using Movies.Models;
using Movies.Repositories.IRepositories;

namespace Movies.Repositories
{
    public class RoleRepository : Repository<IdentityRole>, IRoleRepository
    {
        private readonly ApplicationDbContext _context;
        public RoleRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

       
    }
}
