using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movies.Data;
using Movies.Repositories;
using Movies.Repositories.IRepositories;

namespace Movies.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class MovieController : Controller
    {
        private readonly IMovieRepository _movieRepository = new MovieRepository();
        private readonly IActorRepository _actorRepository = new ActorRepository();
        
        public IActionResult Index(int Id)
        {
            var movie = _movieRepository.GetAll(includes: [m => m.Category, m => m.Cinema, m => m.ActorMovies]).FirstOrDefault(m => m.Id == Id);
            var actors = _actorRepository.GetAll();
            ViewBag.Actors = actors;
            return View(movie);
        }
    }
}
