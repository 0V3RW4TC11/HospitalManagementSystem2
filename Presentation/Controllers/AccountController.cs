using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Operations;
using Presentation.Helpers;
using Presentation.Models;
using Presentation.Models.Account;
using Services.Abstractions;

namespace Presentation.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IServiceManager manager)
        {
            _accountService = manager.AccountService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _accountService.LoginAsync(
                       model.UserName,
                       model.Password,
                       model.IsPersistant,
                       false);

                    return UrlHelper.Redirect(this, returnUrl);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _accountService.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword(Guid id)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(AccountChangePasswordViewModel model)
        {
            throw new NotImplementedException();
        }

        [Authorize(Roles = Constants.AuthRoles.Admin)]
        [HttpGet]
        public async Task<IActionResult> ResetPassword(Guid id, string? returnUrl)
        {
            throw new NotImplementedException();
        }

        [Authorize(Roles = Constants.AuthRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(object model, string? returnUrl)
        {
            throw new NotImplementedException();
        }

        [Authorize(Roles = Constants.AuthRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> SetAccountLockout(Guid id, bool enabled)
        {
            try
            {
                await _accountService.SetLockoutAsync(id, enabled);
                return Ok();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
