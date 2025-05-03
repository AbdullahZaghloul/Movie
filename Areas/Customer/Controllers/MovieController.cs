using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movies.Data;

namespace Movies.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class MovieController : Controller
    {
        private readonly ApplicationDbContext _Context = new();
        public IActionResult Index(int Id)
        {
            var movie = _Context.Movies.Include(m=>m.Category).Include(m=>m.Cinema).Include(m=>m.ActorMovies).FirstOrDefault(m => m.Id == Id);
            var actors = _Context.Actors.ToList();
            ViewBag.Actors = actors;
            return View(movie);
        }
    }
}
