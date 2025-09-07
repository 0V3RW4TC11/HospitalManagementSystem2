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
        public async Task<IActionResult> Index(int? page)
        {
            int pageNum = page ?? 1;
            int pageSize = 10;

            try
            {
                var admins = await _adminService.Admins(pageNum, pageSize);
                var pagedAdmins = admins.List
                    .Select(a => a.Adapt<AdminListItemViewModel>())
                    .ToPagedList(pageNum, pageSize, admins.TotalCount);
                return View(pagedAdmins);
            }
            catch (Exception ex)
            {
                throw;
            }
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
                    var adminCreateDto = model.Adapt<AdminCreateDto>();
                    await _adminService.CreateAsync(adminCreateDto);
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
        public async Task<IActionResult> Manage(Guid id)
        {
            try
            {
                var model = await GetAdminDetailsViewModel(id);
                return View(model);
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
                var model = await GetAdminDetailsViewModel(id);
                return View(model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AdminDetailsViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var adminDto = model.Adapt<AdminDto>();
                    await _adminService.UpdateAsync(adminDto);
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
                    var adminDto = model.Adapt<AdminDto>();
                    adminDto.Id = await _identityService.GetLoggedInUserId();
                    await _adminService.UpdateAsync(adminDto);
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

        private async Task<AdminDetailsViewModel> GetAdminDetailsViewModel(Guid id)
        {
            var admin = await _adminService.GetByIdAsync(id);
            var model = admin.Adapt<AdminDetailsViewModel>();
            model.Username = await _identityService.GetUserNameAsync(id);
            model.IsLockedOut = await _identityService.IsLockedOut(id);

            return model;
        }

        private async Task<AdminProfileViewModel> GetAdminProfileViewModel(Guid id)
        {
            var admin = await _adminService.GetByIdAsync(id);
            var model = admin.Adapt<AdminProfileViewModel>();
            model.Username = await _identityService.GetUserNameAsync(id);
            return model;
        }
    }
}
