using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Movies.Models;
using Movies.Models.ViewModels;
using Movies.Utility;

namespace Movies.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _IEmailSender;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender iEmailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _IEmailSender = iEmailSender;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
            {
                return View(registerVM);
            }

            ApplicationUser applicationUser = new()
            {
                UserName = registerVM.UserName,
                Email = registerVM.Email,
                Address = registerVM.Address,
            };

            var result = await _userManager.CreateAsync(applicationUser, registerVM.Password);

            if (result.Succeeded)
            {
                string token = await _userManager.GenerateEmailConfirmationTokenAsync(applicationUser);

                var confirmationLink = Url.Action("ConfirmEmail", "Account", new { area = "Identity", applicationUser.Id, token }, Request.Scheme);

                await _IEmailSender.SendEmailAsync(applicationUser.Email, "Confirmation Email"
                    , $"<h1>Confirm Your Account By Click <a href='{confirmationLink}'>Here</a></h1>");

                TempData["Notification"] = "Add Account successfully, Confirm Your Account";

                return RedirectToAction("Index", "Home", new { area = "Customer" });
            }
            else
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Description);
                }
            }

            return View(registerVM);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
            {
                return View(loginVM);
            }

            var applicationUser = await _userManager.FindByEmailAsync(loginVM.UserNameOREmail);

            if (applicationUser is null)
            {
                applicationUser = await _userManager.FindByNameAsync(loginVM.UserNameOREmail);
            }

            if (applicationUser is not null)
            {

                var result = await _userManager.CheckPasswordAsync(applicationUser, loginVM.Password);

                if (result)
                {
                    await _signInManager.SignInAsync(applicationUser, loginVM.RememberMe);

                    TempData["Notification"] = "Login successfully";

                    return RedirectToAction("Index", "Home", new { area = "Customer" });
                }

                ModelState.AddModelError("Password", "Invalid Password");
                return View(loginVM);
            }

            ModelState.AddModelError("UserNameOREmail", "Invalid User Name Or Email");
            return View(loginVM);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            TempData["Notification"] = "Logout successfully";

            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }

        public async Task<IActionResult> ConfirmEmail(string Id, string token)
        {
            var applicationUser = await _userManager.FindByIdAsync(Id);

            if (applicationUser is not null)
            {

                var result = await _userManager.ConfirmEmailAsync(applicationUser, token);

                if (result.Succeeded)
                {
                    TempData["Notification"] = "Confirmed Email successfully";

                    return RedirectToAction("Index", "Home", new { area = "Customer" });
                }
                else
                {
                    TempData["Notification-error"] = String.Join(", ", result.Errors.Select(e => e.Description));
                }
            }

            return BadRequest();
        }


        public IActionResult ResendEmailConfirmation()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResendEmailConfirmation(ResendEmailConfirmationVM resendEmailConfirmationVM)
        {
            if (!ModelState.IsValid)
            {
                return View(resendEmailConfirmationVM);
            }

            var applicationUser = await _userManager.FindByEmailAsync(resendEmailConfirmationVM.UserNameOREmail);

            if (applicationUser is null)
            {
                applicationUser = await _userManager.FindByNameAsync(resendEmailConfirmationVM.UserNameOREmail);
            }

            if (applicationUser is not null)
            {
                if (applicationUser.EmailConfirmed)
                {
                    TempData["Notification-error"] = "Already Confirmed";
                }
                else
                {
                    string token = await _userManager.GenerateEmailConfirmationTokenAsync(applicationUser);

                    var confirmationLink = Url.Action("ConfirmEmail", "Account", new { area = "Identity", applicationUser.Id, token }, Request.Scheme);

                    await _IEmailSender.SendEmailAsync(applicationUser.Email, "Resend Confirmation Email"
                        , $"<h1>Confirm Your Account By Click <a href='{confirmationLink}'>Here</a></h1>");

                    TempData["Notification"] = "Send Email successfully";
                }

                return RedirectToAction("Index", "Home", new { area = "Customer" });
            }

            ModelState.AddModelError("UserNameOREmail", "Invalid User Name Or Email");
            return View(resendEmailConfirmationVM);
        }

    }
}
