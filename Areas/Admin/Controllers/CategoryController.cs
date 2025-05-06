using Microsoft.AspNetCore.Mvc;
using Movies.Data;
using Movies.Models;
using Movies.Repositories;
using Movies.Repositories.IRepositories;
using System.Threading.Tasks;

namespace Movies.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository = new CategoryRepository();
        public IActionResult Index()
        {
            var categories =  _categoryRepository.GetAll();
            return View(categories.ToList());
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                
                await _categoryRepository.AddAsync(category);
                await _categoryRepository.CommitAsync();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        public IActionResult Edit(int Id)
        {
            var category = _categoryRepository.Get(exception: [c => c.Id == Id]);
            if (category is not null)
            {
                return View(category);
            }
            return RedirectToAction("NotFound", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                
                _categoryRepository.Update(category);
                await _categoryRepository.CommitAsync();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        public async Task<IActionResult> Delete(int Id)
        {
            var category = _categoryRepository.Get(exception: [c => c.Id == Id]);
            if (category is not null)
            {
                _categoryRepository.Delete(category);
                await _categoryRepository.CommitAsync();
                return RedirectToAction("Index");
            }
            return RedirectToAction("NotFound", "Home");
        }
    }
}
