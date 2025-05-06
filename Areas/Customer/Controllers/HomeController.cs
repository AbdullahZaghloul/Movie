using System.Diagnostics;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movies.Data;
using Movies.Models;
using Movies.Models.ViewModels;
using Movies.Repositories;
using Movies.Repositories.IRepositories;

namespace Movies.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMovieRepository _movieRepository = new MovieRepository();
        private readonly ICinemaRepository _cinemaRepository = new CinemaRepository();
        private readonly ICategoryRepository _categoryRepository = new CategoryRepository();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(int categoryId,int cinemaId,int? minPrice,int? maxPrice,int page=1)
        {
            IQueryable<Movie> movies = _movieRepository.GetAll(includes: [m => m.Category,m=>m.Cinema]);
            var categories = _categoryRepository.GetAll();
            var cinemas = _cinemaRepository.GetAll();

            if(categoryId > 0)
            {
                movies = movies.Where(m => m.CategoryId == categoryId);
            }
            if(cinemaId > 0)
            {
                movies = movies.Where(m => m.CinemaId == cinemaId);
            }
            if (minPrice > 0)
            {
                movies = movies.Where(m => m.Price >= minPrice);
            }

            if (maxPrice > 0)
            {
                movies = movies.Where(m => m.Price <= maxPrice);
            }

            int totalNumOfPages = (int)Math.Ceiling(movies.Count() / 8.0);
            movies = movies.Skip((page-1) *8).Take(8);

            var movieWithCatgoiesWithCinemas = new MovieWithCategoriesWithCinemasVM()
            {
                Movies = movies.ToList(),
                Categories = categories.ToList(),
                Cinemas = cinemas.ToList(),
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                CategoryId = categoryId,
                CinemaId = cinemaId,
                TotalNumOfPages=totalNumOfPages
            };

            return View(movieWithCatgoiesWithCinemas);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
