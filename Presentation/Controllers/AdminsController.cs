using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models.Admin;
using Services.Abstractions;
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
                    await _adminService.CreateAsync(model.Dto);
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
                var model = new AdminManageViewModel(
                    await _adminService.GetByIdAsync(id),
                    await _identityService.GetUserNameAsync(id),
                    await _identityService.IsLockedOutAsync(id));
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
                    await _adminService.UpdateAsync(model.Dto);
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
                var id = await _identityService.GetLoggedInUserId();
                var model = new AdminProfileViewModel(
                    await _adminService.GetByIdAsync(id),
                    await _identityService.GetUserNameAsync(id));
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
                    var id = await _identityService.GetLoggedInUserId();
                    await _adminService.UpdateAsync(model.Dto(id));
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
                var model = new AdminManageViewModel(
                    await _adminService.GetByIdAsync(id),
                    await _identityService.GetUserNameAsync(id),
                    await _identityService.IsLockedOutAsync(id));

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
                var id = await _identityService.GetLoggedInUserId();

                var model = new AdminProfileViewModel(
                    await _adminService.GetByIdAsync(id),
                    await _identityService.GetUserNameAsync(id));

                return View(model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
