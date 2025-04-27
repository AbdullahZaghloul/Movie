using Microsoft.AspNetCore.Mvc;
using Movies.Data;
using Movies.Models;

namespace Movies.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CinemaController : Controller
    {
        private readonly ApplicationDbContext _context = new();
        public IActionResult Index()
        {
            var cinemas = _context.Cinemas;
            return View(cinemas.ToList());
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Cinema cinema)
        {
            if (ModelState.IsValid)
            {
                _context.Cinemas.Add(cinema);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cinema);
        }

        public IActionResult Edit(int Id)
        {
            var cinema = _context.Cinemas.Find(Id);
            if (cinema is not null)
            {
                return View(cinema);
            }
            return RedirectToAction("NotFound", "Home");
        }
        
        [HttpPost]
        public IActionResult Edit(Cinema cinema)
        {
            if (ModelState.IsValid)
            {
                _context.Cinemas.Update(cinema);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cinema);
        }

        public IActionResult Delete(int Id)
        {
            var cinema = _context.Cinemas.Find(Id);
            if (cinema is not null)
            {
                _context.Cinemas.Remove(cinema);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return RedirectToAction("NotFound", "Home");
        }
    }
}
