using DataTransfer.Admin;
using Domain.Constants;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;
using Presentation.Models.Admin;
using Services.Abstractions;
using X.PagedList.Extensions;
using static Presentation.Helpers.TryHelper;
using static Presentation.Helpers.ModelHelper;

namespace Presentation.Controllers
{
    [Authorize(Roles = AuthRoles.Admin)]
    public class AdminsController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly IAccountDictionary _accountDictionary;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminsController(IServiceManager serviceManager, UserManager<IdentityUser> userManager)
        {
            _adminService = serviceManager.AdminService;
            _accountDictionary = serviceManager.AccountDictionary;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? page)
        {
            int pageNum = page ?? 1;
            int pageSize = 10;

            return await Try(async () =>
            {
                var admins = await _adminService.GetAdminsAsync(pageNum, pageSize);
                var pagedAdmins = admins.List
                    .Select(a => a.Adapt<AdminListItemViewModel>())
                    .ToPagedList(pageNum, pageSize, admins.TotalCount);

                return View(pagedAdmins);
            });
        }

        [HttpGet]
        public IActionResult Administration()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            return await Try(async () =>
            {
                var model = await GetAdminDetailsViewModel(id);
                return View(model);
            });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            return await Try(async () =>
            {
                var model = await GetAdminDetailsViewModel(id);
                return View(model);
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AdminDetailsViewModel model)
        {
            return await Try<IActionResult>(async () =>
            {
                if (ModelState.IsValid is false)
                    return View(model);

                var adminDto = model.Adapt<AdminDto>()
                    ?? throw new Exception("Failure to adapt ViewModel to AdminDto");

                await _adminService.UpdateAsync(adminDto);

                return RedirectToAction(nameof(Details), new { id = model.Id });
            });
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword(Guid id)
        {
            return await Try(async () =>
            {
                var userName = await GetUserName(id);
                var model = new ChangePasswordViewModel { UserId = id, UserName = userName };
                return View(model);
            });
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            return await Try<IActionResult>(async () =>
            {
                if (ModelState.IsValid)
                {
                    var identity = await GetIdentityUserByUserId(model.UserId);
                    var result = await _userManager.ChangePasswordAsync(identity, model.OldPassword, model.NewPassword);

                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(Details), new { id = model.UserId });
                    }
                    else
                    {
                        var descriptions = AdaptIdentityErrorsToStrings(result.Errors);
                        AddErrorsToModel(ModelState, descriptions);
                    }
                }
                
                return View(model);
            });
        }

        private async Task<AdminDetailsViewModel> GetAdminDetailsViewModel(Guid id)
        {
            var admin = await _adminService.GetByIdAsync(id);
            var model = admin.Adapt<AdminDetailsViewModel>();
            model.Username = await GetUserName(id);

            return model;
        }

        private async Task<string> GetUserName(Guid id)
        {
            var identity = await GetIdentityUserByUserId(id);
            var userName = identity.UserName
                    ?? throw new Exception("Failed to retrieve UserName for Identity Id: " + identity.Id);

            return userName;
        }

        private async Task<IdentityUser> GetIdentityUserByUserId(Guid id)
        {
            var identityId = await _accountDictionary.GetIdentityIdByUserId(id);
            var identity = await _userManager.FindByIdAsync(identityId)
                ?? throw new Exception("Failed to retrieve Identity for User Id: " + id);

            return identity;
        }
    }
}
