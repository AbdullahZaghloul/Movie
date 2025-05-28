using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Movies.Models;
using Movies.Models.ViewModels;
using Movies.Repositories.IRepositories;
using Movies.Utility;
using NuGet.Common;
using System.Security.Permissions;
using System.Threading.Tasks;

namespace Movies.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IApplicationUserOTPRepository _applicationUserOTPRepository;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _IEmailSender;

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender iEmailSender,
            IApplicationUserOTPRepository applicationUserOTPRepository,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _IEmailSender = iEmailSender;
            _applicationUserOTPRepository = applicationUserOTPRepository;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Register()
        {
            if (_roleManager.Roles.IsNullOrEmpty())
            {
                await _roleManager.CreateAsync(new(SD.Admin));
                await _roleManager.CreateAsync(new(SD.SuperAdmin));
                await _roleManager.CreateAsync(new(SD.Customer));
                await _roleManager.CreateAsync(new(SD.Company));
            }
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
                await _userManager.AddToRoleAsync(applicationUser, SD.Customer);

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

            if (applicationUser is not null && applicationUser.LockoutEnabled)
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

        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordVM forgetPasswordVM)
        {
            if (!ModelState.IsValid)
            {
                return View(forgetPasswordVM);
            }
            var applicationUser = await _userManager.FindByEmailAsync(forgetPasswordVM.UserNameOrEmail);
            if (applicationUser is null)
            {
                applicationUser = await _userManager.FindByNameAsync(forgetPasswordVM.UserNameOrEmail);
            }
            if (applicationUser is not null)
            {
                var otpInDb = _applicationUserOTPRepository
                    .Get(exception: [e => e.ApplicationUserId == applicationUser.Id]);

                if (otpInDb is null ||(otpInDb is not null && otpInDb.RealseDate.AddMinutes(10) < DateTime.UtcNow))
                {
                    var otp = new Random().Next(1000, 9999);
                    var result = _IEmailSender.SendEmailAsync(applicationUser.Email, "Forget Password"
                        , $"<h1>Reset password using this Number {otp} </h1>");
                    TempData["Notification"] = "the otp email send successfully";
                    TempData["ValidationToken"] = Guid.NewGuid().ToString();
                    var token =await _userManager.GeneratePasswordResetTokenAsync(applicationUser);
                    await _applicationUserOTPRepository.AddAsync(
                        new ApplicationUserOTP
                        {
                            ApplicationUser = applicationUser,
                            ApplicationUserId = applicationUser.Id,
                            OTP = otp,
                            RealseDate = DateTime.UtcNow,
                            ExpireDate = DateTime.UtcNow.AddMinutes(10)
                        });
                    await _applicationUserOTPRepository.CommitAsync();
                    return RedirectToAction("ResetPassword", "Account", new { area = "Identity", ApplicationUserId = applicationUser.Id, Token = token.ToString() });

                }
                ModelState.AddModelError(string.Empty, "there is an error");
                return View(forgetPasswordVM);

            }
            ModelState.AddModelError(string.Empty, "the user name or email is not correct");
            return View(forgetPasswordVM);

        }
        public IActionResult ResetPassword()
        {
            if (TempData["ValidationToken"] is not null)
            {
                return View();
            }
            return BadRequest();
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM resetPasswordVM)
        {
            if (!ModelState.IsValid)
            {
                return View(resetPasswordVM);
            }
            var applicationUser = await _userManager.FindByIdAsync(resetPasswordVM.ApplicationUserId);
            if (applicationUser is not null)
            {
                var otpInDb =  _applicationUserOTPRepository
                    .Get(exception: [e => e.ApplicationUserId == applicationUser.Id]);

                if (resetPasswordVM.OTP == otpInDb.OTP && otpInDb.ExpireDate >= DateTime.UtcNow)
                {
                    var result = await _userManager.ResetPasswordAsync(applicationUser, resetPasswordVM.Token, resetPasswordVM.Password);

                    if (result.Succeeded)
                    {
                        TempData["Notification"] = "password reset successfully";
                        return RedirectToAction("Index", "Home", new { area = "Customer" });
                    }
                    else
                    {
                        foreach (var item in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, item.Description);
                        }
                    }

                }
                ModelState.AddModelError(string.Empty, "the otp is not correct or valid");
                return View(resetPasswordVM);
            }
            return BadRequest();
        }
    }
}

