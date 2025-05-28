using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Data;
using Movies.Models;
using Movies.Repositories;
using Movies.Repositories.IRepositories;
using Movies.Utility;
using System.Threading.Tasks;

namespace Movies.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CinemaController : Controller
    {
        //private readonly ApplicationDbContext _context = new();
        private readonly ICinemaRepository _cinemaRepository;

        public CinemaController(ICinemaRepository cinemaRepository)
        {
            _cinemaRepository = cinemaRepository;
        }
        [Authorize(Roles = $"{SD.SuperAdmin},{SD.Admin},{SD.Customer}")]

        public IActionResult Index()
        {
            var cinemas = _cinemaRepository.GetAll();
            return View(cinemas.ToList());
        }
        public IActionResult Create()
        {
            return View();
        }
        [Authorize(Roles = $"{SD.SuperAdmin},{SD.Admin}")]

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
        [Authorize(Roles = $"{SD.SuperAdmin},{SD.Admin}")]

        public IActionResult Edit(int Id)
        {
            var cinema = _cinemaRepository.Get(exception: [c => c.Id == Id]);
            if (cinema is not null)
            {
                return View(cinema);
            }
            return RedirectToAction("NotFound", "Home");
        }
        [Authorize(Roles = $"{SD.SuperAdmin},{SD.Admin}")]

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
        [Authorize(Roles = $"{SD.SuperAdmin},{SD.Admin}")]

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
