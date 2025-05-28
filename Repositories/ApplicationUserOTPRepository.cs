using Movies.Data;
using Movies.Models;
using Movies.Models.ViewModels;
using Movies.Repositories.IRepositories;

namespace Movies.Repositories
{
    public class ApplicationUserOTPRepository : Repository<ApplicationUserOTP>, IApplicationUserOTPRepository
    {
        private readonly ApplicationDbContext _context;
        public ApplicationUserOTPRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

       
    }
}
