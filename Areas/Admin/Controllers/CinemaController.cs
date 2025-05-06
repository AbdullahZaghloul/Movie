using Microsoft.AspNetCore.Mvc;
using Movies.Data;
using Movies.Models;
using Movies.Repositories;
using Movies.Repositories.IRepositories;
using System.Threading.Tasks;

namespace Movies.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CinemaController : Controller
    {
        //private readonly ApplicationDbContext _context = new();
        private readonly ICinemaRepository _cinemaRepository=new CinemaRepository();
        public IActionResult Index()
        {
            var cinemas = _cinemaRepository.GetAll();
            return View(cinemas.ToList());
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Cinema cinema)
        {
            if (ModelState.IsValid)
            {
                
                await _cinemaRepository.AddAsync(cinema);
                await _cinemaRepository.CommitAsync();
                return RedirectToAction("Index");
            }
            return View(cinema);
        }

        public IActionResult Edit(int Id)
        {
            var cinema = _cinemaRepository.Get(exception: [c => c.Id == Id]);
            if (cinema is not null)
            {
                return View(cinema);
            }
            return RedirectToAction("NotFound", "Home");
        }
        
        [HttpPost]
        public async Task<IActionResult> Edit(Cinema cinema)
        {
            if (ModelState.IsValid)
            {
                _cinemaRepository.Update(cinema);
                await _cinemaRepository.CommitAsync();
                return RedirectToAction("Index");
            }
            return View(cinema);
        }

        public async Task<IActionResult> Delete(int Id)
        {
            var cinema = _cinemaRepository.Get(exception: [c => c.Id == Id]);
            if (cinema is not null)
            {
                _cinemaRepository.Delete(cinema);
                await _cinemaRepository.CommitAsync();
                return RedirectToAction("Index");
            }
            return RedirectToAction("NotFound", "Home");
        }
    }
}
