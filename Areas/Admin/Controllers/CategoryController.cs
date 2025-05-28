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
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        [Authorize(Roles =$"{SD.SuperAdmin},{SD.Admin},{SD.Customer}")]
        public IActionResult Index()
        {
            var categories =  _categoryRepository.GetAll();
            return View(categories.ToList());
        }
        [Authorize(Roles = $"{SD.SuperAdmin},{SD.Admin}")]

        public IActionResult Create()
        {
            return View();
        }
        [Authorize(Roles = $"{SD.SuperAdmin},{SD.Admin}")]

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
        [Authorize(Roles = $"{SD.SuperAdmin},{SD.Admin}")]

        public IActionResult Edit(int Id)
        {
            var category = _categoryRepository.Get(exception: [c => c.Id == Id]);
            if (category is not null)
            {
                return View(category);
            }
            return RedirectToAction("NotFound", "Home");
        }
        [Authorize(Roles = $"{SD.SuperAdmin},{SD.Admin}")]

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
        [Authorize(Roles = $"{SD.SuperAdmin},{SD.Admin}")]

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
