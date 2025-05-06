using Microsoft.AspNetCore.Mvc;
using Movies.Data;
using Movies.Repositories;
using Movies.Repositories.IRepositories;

namespace Movies.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ActorController : Controller
    {
        private readonly IActorRepository _actorRepository = new ActorRepository();
        public IActionResult Index(int Id)
        {
            var actor = _actorRepository.Get(exception:[a=>a.Id == Id]);
            return View(actor);
        }
    }
}
