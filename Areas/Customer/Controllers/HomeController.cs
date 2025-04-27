using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movies.Data;
using Movies.Models;
using Movies.Models.ViewModels;

namespace Movies.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _Context = new();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(int categoryId,int cinemaId,int? minPrice,int? maxPrice,int page=1)
        {
            IQueryable<Movie> movies = _Context.Movies.Include(m=>m.Category);
            var categories = _Context.Categories;
            var cinemas = _Context.Cinemas;

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
