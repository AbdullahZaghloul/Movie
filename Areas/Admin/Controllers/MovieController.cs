using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movies.Data;
using Movies.Models;
using Movies.Models.ViewModels;
using Movies.Repositories;
using Movies.Repositories.IRepositories;
using Movies.Utility;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Movies.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MovieController : Controller
    {
        //private readonly ApplicationDbContext _context = new();
        private readonly IMovieRepository _movieRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICinemaRepository _cinemaRepository;
        private readonly IActorRepository _actorRepository;
        private readonly IMoveActorRepository _moveActorRepository;

        public MovieController(IMovieRepository movieRepository, ICategoryRepository categoryRepository
            , ICinemaRepository cinemaRepository, IActorRepository actorRepository, IMoveActorRepository moveActorRepository)
        {
            _movieRepository = movieRepository;
            _categoryRepository = categoryRepository;
            _cinemaRepository = cinemaRepository;
            _actorRepository = actorRepository;
            _moveActorRepository = moveActorRepository;
        }
        [Authorize(Roles = $"{SD.SuperAdmin},{SD.Admin},{SD.Customer}")]

        public IActionResult Index()
        {
            var movies = _movieRepository.GetAll();
            return View(movies.ToList());
        }
        [Authorize(Roles = $"{SD.SuperAdmin},{SD.Admin}")]

        [HttpGet]
        public IActionResult Create()
        {
            var vm = new MovieWithCategoryWithCinemaVM2
            {
                Movie = new Movie(),
                Categories = _categoryRepository.GetAll().ToList(),
                Cinemas = _cinemaRepository.GetAll().ToList(),
                Actors = _actorRepository.GetAll().ToList()
            };

            return View(vm);
        }
        [Authorize(Roles = $"{SD.SuperAdmin},{SD.Admin}")]

        [HttpPost]
        public async Task<IActionResult> Create(MovieWithCategoryWithCinemaVM2 vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            // Process image files
            if (vm.ImageFiles != null && vm.ImageFiles.Count > 0)
            {
                vm.Movie.ImgUrl = new List<string>();
                foreach (var imageFile in vm.ImageFiles)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\movies", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    vm.Movie.ImgUrl.Add(fileName);
                }
            }

            // Process trailer file
            if (vm.TrailerFile != null && vm.TrailerFile.Length > 0)
            {
                var trailerName = Guid.NewGuid() + Path.GetExtension(vm.TrailerFile.FileName);
                var trailerPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\videos", trailerName);

                using (var stream = new FileStream(trailerPath, FileMode.Create))
                {
                    await vm.TrailerFile.CopyToAsync(stream);
                }
                vm.Movie.TrailerUrl = trailerName;
            }

            // Save movie
            await _movieRepository.AddAsync(vm.Movie);
            await _movieRepository.CommitAsync();

            // Save actor relationships
            if (vm.SelectedActorIds != null)
            {
                foreach (var actorId in vm.SelectedActorIds)
                {
                    await _moveActorRepository.AddAsync(new ActorMovie
                    {
                        MovieId = vm.Movie.Id,
                        ActorId = actorId
                    });
                }
                await _moveActorRepository.CommitAsync();
            }

            return RedirectToAction("Index");
        }
        [Authorize(Roles = $"{SD.SuperAdmin},{SD.Admin}")]

        public IActionResult Edit(int id)
        {
            var movie = _movieRepository.Get(exception: [m => m.Id == id]);

            if (movie is not null)
            {
                var categories = _categoryRepository.GetAll();
                //var cinemas = _context.Cinemas;
                var cinemas = _cinemaRepository.GetAll();
                var actors = _actorRepository.GetAll();

                MovieWithCategoryWithCinemaVM2 movieWithCategoryWithCinemaVM2 = new()
                {
                    Movie = movie,
                    Categories = categories.ToList(),
                    Cinemas = cinemas.ToList(),
                    Actors = actors.ToList(),
                };

                return View(movieWithCategoryWithCinemaVM2);
            }

            return RedirectToAction("NotFoundPage", "Home");
        }
        [Authorize(Roles = $"{SD.SuperAdmin},{SD.Admin}")]

        [HttpPost]
        public async Task<IActionResult> Edit(MovieWithCategoryWithCinemaVM2 vm)
        {
            var movieInDb = _movieRepository.Get(exception: [m => m.Id == vm.Movie.Id], Tracked: false);

            if (ModelState.IsValid && movieInDb != null)
            {
                if (vm.ImageFiles != null && vm.ImageFiles.Count > 0)
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
                if(vm.TrailerFile!=null && vm.TrailerFile.Length > 0)
                {
                    var FileName = Guid.NewGuid().ToString() + Path.GetExtension(vm.TrailerFile.FileName);
                    var FilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Videos", FileName);

                    using (var stream = System.IO.File.Create(FilePath))
                    {
                        await vm.TrailerFile.CopyToAsync(stream);
                    }

                    vm.Movie.TrailerUrl = FileName;


                    // Delete old trialer from wwwroot

                    var oldFileName = movieInDb.TrailerUrl;
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Videos", oldFileName);

                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                    // Update img in Db
                }
                else
                {
                    // Save the old product img
                    vm.Movie.ImgUrl = movieInDb.ImgUrl;
                    vm.Movie.TrailerUrl = movieInDb.TrailerUrl;
                }

                _movieRepository.Update(vm.Movie);
                await _movieRepository.CommitAsync();

                return RedirectToAction(nameof(Index));
            }

            var categories = _categoryRepository.GetAll();
            var cinemas = _cinemaRepository.GetAll();
            var actors = _actorRepository.GetAll();
            MovieWithCategoryWithCinemaVM2 movieWithCategoryWithCinemaVM2 = new()
            {
                Movie = vm.Movie,
                Categories = categories.ToList(),
                Cinemas = cinemas.ToList(),
                Actors = actors.ToList(),
            };

            return View(movieWithCategoryWithCinemaVM2);
        }
        [Authorize(Roles = $"{SD.SuperAdmin},{SD.Admin}")]

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
