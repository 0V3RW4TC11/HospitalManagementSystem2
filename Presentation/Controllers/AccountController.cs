using Commands.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Helpers;
using Presentation.ViewModels.Account;

namespace Presentation.Controllers
{
    public class AccountController : Controller
    {
        private readonly ISender _sender;

        public AccountController(ISender sender) => _sender = sender;

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(AccountLoginViewModel model, string? returnUrl)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _sender.Send(new LoginCommand(
                        model.UserName,
                        model.Password,
                        model.IsPersistant,
                        false));

                    return UrlHelper.RedirectOrDefaultAction(
                        this, 
                        returnUrl, 
                        nameof(HomeController).Replace("Controller", ""), 
                        nameof(HomeController.Index));
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
            await _sender.Send(new LogoutCommand());
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
                await _sender.Send(new SetLockOutCommand(id, enabled));
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
