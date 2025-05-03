using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movies.Data;
using Movies.Models;
using Movies.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Movies.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MovieController : Controller
    {
        private readonly ApplicationDbContext _context = new();
        public IActionResult Index()
        {
            var movies = _context.Movies;
            return View(movies.ToList());
        }

       public IActionResult Create()
        {
            var categories = _context.Categories;
            var cinemas = _context.Cinemas;

            var movieWithCategoriesWithCienams2 = new MovieWithCategoryWithCinemaVM2()
            {
                Movie = new Movie(),
                Categories = categories.ToList(),
                Cinemas = cinemas.ToList(),

            };

            return View(movieWithCategoriesWithCienams2);
        }
        [HttpPost]
        public async Task<IActionResult> Create(MovieWithCategoryWithCinemaVM2 vm)
        {

            if (ModelState.IsValid && vm.ImageFiles != null && vm.ImageFiles.Count > 0 && vm.Trailer != null && vm.Trailer.Length > 0)
            {
                foreach (var imgUrl in vm.ImageFiles)
                {
                    var FileName = Guid.NewGuid().ToString() + Path.GetExtension(imgUrl.FileName);
                    var FilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\movies", FileName);

                    using (var stream = System.IO.File.Create(FilePath))
                    {
                        await imgUrl.CopyToAsync(stream);
                    }

                    vm.Movie.ImgUrl.Add(FileName);
                }

                var TrailerName = Guid.NewGuid().ToString() + Path.GetExtension(vm.Trailer.FileName);
                var TrailerPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Videos", TrailerName);

                using (var stream = System.IO.File.Create(TrailerPath))
                {
                    await vm.Trailer.CopyToAsync(stream);
                }

                vm.Movie.TrailerUrl = TrailerName;

                await _context.Movies.AddAsync(vm.Movie);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            var categories = _context.Categories;
            var cinemas = _context.Cinemas;

            var MovieWithCategoryWithCinemaVM2 = new MovieWithCategoryWithCinemaVM2()
            {
                Movie = vm.Movie,
                Categories = categories.ToList(),
                Cinemas = cinemas.ToList()
            };

            return View(MovieWithCategoryWithCinemaVM2);


        }

    }
}
