using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Movies.Models;
using Movies.Models.ViewModels;
using Movies.Repositories;
using Movies.Repositories.IRepositories;
using System.Threading.Tasks;

namespace Movies.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly IApplicationUserRepository _applicationUserRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRoleRepository _roleRepository;

        public UserController(IApplicationUserRepository applicationUserRepository, UserManager<ApplicationUser> userManager, IRoleRepository roleRepository)
        {
            _applicationUserRepository = applicationUserRepository;
            _userManager = userManager;
            _roleRepository = roleRepository;
        }

        public async Task<IActionResult> Index()
        {
            var users = _applicationUserRepository.GetAll().ToList();
            var dictionary = new Dictionary<ApplicationUser, string>();
            foreach (var user in users) {
                var userRoles = await _userManager.GetRolesAsync(user);
                dictionary.Add(user, string.Join(", ", userRoles));
            }
            return View(dictionary);
        }
        public async Task<IActionResult> ChangeRole(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);
            var roles = _roleRepository.GetAll();
            if(user is not null)
            {
                var UserWithRoleVM = new UserWithRolesVM()
                {
                    ApplicationUser = user,
                    IdentityRoles = roles.ToList()
                };
                return View(UserWithRoleVM);
            }
            return BadRequest();
        }
        [HttpPost]
        public async Task<IActionResult> ChangeRole(UserWithRolesVM userWithRolesVM , string role)
        {
            if (!ModelState.IsValid)
            {
                userWithRolesVM.IdentityRoles = _roleRepository.GetAll().ToList();
                return View(userWithRolesVM);
            }

            var user = await _userManager.FindByIdAsync(userWithRolesVM.ApplicationUser.Id);
            if(user is not null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, userRoles);
                await _userManager.AddToRoleAsync(user, role);
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> BlockUnBlock(string Id)
        {
            var user =await _userManager.FindByIdAsync(Id);
            if(user is not null)
            {
                user.LockoutEnabled = !user.LockoutEnabled;
                if (!user.LockoutEnabled)
                {
                    user.LockoutEnd = DateTime.UtcNow.AddDays(1);
                }
                else
                {
                    user.LockoutEnd = null;
                }
                await _userManager.UpdateAsync(user);
                return RedirectToAction("Index");
            }
            return BadRequest();
        }
    }
}
