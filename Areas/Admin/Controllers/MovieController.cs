using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movies.Data;
using Movies.Models;
using Movies.Models.ViewModels;
using Movies.Repositories;
using Movies.Repositories.IRepositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Movies.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MovieController : Controller
    {
        //private readonly ApplicationDbContext _context = new();
        private readonly IMovieRepository _movieRepository = new MovieRepository();
        private readonly ICategoryRepository _categoryRepository = new CategoryRepository();
        private readonly ICinemaRepository _cinemaRepository = new CinemaRepository();
        public IActionResult Index()
        {
            var movies = _movieRepository.GetAll();
            return View(movies.ToList());
        }

        public IActionResult Create()
        {
            var categories = _categoryRepository.GetAll();
            var cinemas = _cinemaRepository.GetAll();

            var movieWithCategoriesWithCienams2 = new MovieWithCategoryWithCinemaVM2()
            {
                Movie = new Movie(),
                Categories = categories.ToList(),
                Cinemas = cinemas.ToList(),

            };

            return View(movieWithCategoriesWithCienams2);
        }
        [HttpPost]
        public async Task<IActionResult> Create(Movie movie, List<IFormFile>? ImgUrl, IFormFile TrailerUrl)
        {
            if (ModelState.IsValid && ImgUrl != null && ImgUrl.Count > 0 && TrailerUrl != null && TrailerUrl.Length > 0)
            {
                foreach (var imgUrl in ImgUrl)
                {
                    var FileName = Guid.NewGuid().ToString() + Path.GetExtension(imgUrl.FileName);
                    var FilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\movies", FileName);

                    using (var stream = System.IO.File.Create(FilePath))
                    {
                        await imgUrl.CopyToAsync(stream);
                    }

                    movie.ImgUrl.Add(FileName);
                }

                var TrailerName = Guid.NewGuid().ToString() + Path.GetExtension(TrailerUrl.FileName);
                var TrailerPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Videos", TrailerName);

                using (var stream = System.IO.File.Create(TrailerPath))
                {
                    await TrailerUrl.CopyToAsync(stream);
                }

                movie.TrailerUrl = TrailerName;

                await _movieRepository.AddAsync(movie);
                await _movieRepository.CommitAsync();

                return RedirectToAction("Index");
            }
            var categories = _categoryRepository.GetAll();
            var cinemas = _cinemaRepository.GetAll();

            var MovieWithCategoryWithCinemaVM2 = new MovieWithCategoryWithCinemaVM2()
            {
                Movie = movie,
                Categories = categories.ToList(),
                Cinemas = cinemas.ToList()
            };

            return View(MovieWithCategoryWithCinemaVM2);


        }

        public IActionResult Edit(int id)
        {
            var movie = _movieRepository.Get(exception: [m => m.Id == id]);

            if (movie is not null)
            {
                var categories = _categoryRepository.GetAll();
                //var cinemas = _context.Cinemas;
                var cinemas = _cinemaRepository.GetAll();

                MovieWithCategoryWithCinemaVM2 movieWithCategoryWithCinemaVM2 = new()
                {
                    Movie = movie,
                    Categories = categories.ToList(),
                    Cinemas = cinemas.ToList(),
                };

                return View(movieWithCategoryWithCinemaVM2);
            }

            return RedirectToAction("NotFoundPage", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Movie movie, List<IFormFile>? ImgUrl , IFormFile TrailerUrl)
        {
            var movieInDb = _movieRepository.Get(exception: [m => m.Id == movie.Id], Tracked: false);
            ModelState.Remove("TrailerUrl");

            if (ModelState.IsValid && movieInDb != null)
            {
                if (ImgUrl != null && ImgUrl.Count > 0)
                {
                    foreach (var imgUrl in ImgUrl)
                    {
                        var FileName = Guid.NewGuid().ToString() + Path.GetExtension(imgUrl.FileName);
                        var FilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\movies", FileName);

                        using (var stream = System.IO.File.Create(FilePath))
                        {
                            await imgUrl.CopyToAsync(stream);
                        }

                        // Update img in Db
                        movie.ImgUrl.Add(FileName);

                    }

                    foreach (var imgUrl in movieInDb.ImgUrl) {
                        // Delete old img from wwwroot

                        var oldFileName = imgUrl;
                        var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\movies", oldFileName);

                        if (System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                        }


                    }


                }
                if(TrailerUrl!=null && TrailerUrl.Length > 0)
                {
                    var FileName = Guid.NewGuid().ToString() + Path.GetExtension(TrailerUrl.FileName);
                    var FilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Videos", FileName);

                    using (var stream = System.IO.File.Create(FilePath))
                    {
                        await TrailerUrl.CopyToAsync(stream);
                    }

                    // Update img in Db
                    movie.TrailerUrl = FileName;

                    // Delete old img from wwwroot

                    var oldFileName = movieInDb.TrailerUrl;
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Videos", oldFileName);

                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }
                else
                {
                    // Save the old product img
                    movie.ImgUrl = movieInDb.ImgUrl;
                    movie.TrailerUrl = movieInDb.TrailerUrl;
                }

                _movieRepository.Update(movie);
                await _movieRepository.CommitAsync();

                return RedirectToAction(nameof(Index));
            }

            var categories = _categoryRepository.GetAll();
            var cinemas = _cinemaRepository.GetAll();

            MovieWithCategoryWithCinemaVM2 movieWithCategoryWithCinemaVM2 = new()
            {
                Movie = movie,
                Categories = categories.ToList(),
                Cinemas = cinemas.ToList(),
            };

            return View(movieWithCategoryWithCinemaVM2);
        }
        public IActionResult Delete(int id)
        {
            var movie = _movieRepository.Get(exception: [m=>m.Id==id]);

            if (movie is not null)
            {
                foreach (var imgUrl in movie.ImgUrl)
                {
                    var oldFileNameImg = imgUrl;
                    var oldPathImg = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\movies", oldFileNameImg);

                    if (System.IO.File.Exists(oldFileNameImg))
                    {
                        System.IO.File.Delete(oldPathImg);
                    }
                }

                var oldFileName = movie.TrailerUrl;
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Videos", oldFileName);

                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
                _movieRepository.Delete(movie);
                _movieRepository.CommitAsync();

                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction("NotFoundPage", "Home");

        }

    }
}
