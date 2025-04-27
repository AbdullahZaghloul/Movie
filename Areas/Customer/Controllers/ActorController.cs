using Microsoft.AspNetCore.Mvc;
using Movies.Data;

namespace Movies.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ActorController : Controller
    {
        private readonly ApplicationDbContext _context = new();
        public IActionResult Index(int Id)
        {
            var actor = _context.Actors.Find(Id);
            return View(actor);
        }
    }
}
