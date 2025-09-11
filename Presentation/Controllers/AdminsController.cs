using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models.Admin;
using Services.Abstractions;
using Services.Dtos.Admin;
using X.PagedList.Extensions;

namespace Presentation.Controllers
{
    [Authorize(Roles = Constants.AuthRoles.Admin)]
    public class AdminsController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly IIdentityService _identityService;

        public AdminsController(IServiceManager manager)
        {
            _adminService = manager.AdminService;
            _identityService = manager.IdentityService;
        }

        [HttpGet]
        public IActionResult Dashboard()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AdminCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var dto = model.AdminViewModel.Adapt<AdminCreateDto>();
                    dto.Password = model.PasswordViewModel.Password;
                    await _adminService.CreateAsync(dto);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _adminService.DeleteAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var model = await GetAdminManageViewModel(id);
                return View(model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AdminManageViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var dto = model.AdminViewModel.Adapt<AdminDto>();
                    dto.Id = model.Id;
                    await _adminService.UpdateAsync(dto);
                    return RedirectToAction(nameof(Manage), new { id = model.Id});
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            try
            {
                var userId = await _identityService.GetLoggedInUserId();
                var model = await GetAdminProfileViewModel(userId);
                return View(model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(AdminProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var dto = model.AdminViewModel.Adapt<AdminDto>();
                    dto.Id = await _identityService.GetLoggedInUserId();
                    await _adminService.UpdateAsync(dto);
                    return RedirectToAction(nameof(Profile));
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? page)
        {
            int pageNum = page ?? 1;
            int pageSize = 10;

            try
            {
                var admins = await _adminService.Admins(pageNum, pageSize);
                var pagedResults = admins.List
                    .Select(a => a.Adapt<AdminListItemViewModel>())
                    .ToPagedList(pageNum, pageSize, admins.TotalCount);
                return View(pagedResults);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> Manage(Guid id)
        {
            try
            {
                var model = await GetAdminManageViewModel(id);
                return View(model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            try
            {
                var userId = await _identityService.GetLoggedInUserId();
                var model = await GetAdminProfileViewModel(userId);
                return View(model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task<AdminManageViewModel> GetAdminManageViewModel(Guid id)
        {
            var model = new AdminManageViewModel();
            var admin = await _adminService.GetByIdAsync(id);
            model.Id = admin.Id;
            model.AdminViewModel = admin.Adapt<AdminViewModel>();
            model.Username = await _identityService.GetUserNameAsync(id);
            model.IsLockedOut = await _identityService.IsLockedOutAsync(id);

            return model;
        }

        private async Task<AdminProfileViewModel> GetAdminProfileViewModel(Guid id)
        {
            var model = new AdminProfileViewModel();
            var admin = await _adminService.GetByIdAsync(id);
            model.AdminViewModel = admin.Adapt<AdminViewModel>();
            model.Username = await _identityService.GetUserNameAsync(id);
            return model;
        }
    }
}
