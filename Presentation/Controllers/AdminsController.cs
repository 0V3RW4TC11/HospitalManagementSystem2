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
        private readonly IAccountService _accountService;

        public AdminsController(IServiceManager manager)
        {
            _adminService = manager.AdminService;
            _accountService = manager.AccountService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? page)
        {
            int pageNum = page ?? 1;
            int pageSize = 10;

            try
            {
                var admins = await _adminService.GetAdminsAsync(pageNum, pageSize);
                var pagedAdmins = admins.List
                    .Select(a => a.Adapt<AdminListItemViewModel>())
                    .ToPagedList(pageNum, pageSize, admins.TotalCount);
                return View(pagedAdmins);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return RedirectToAction(nameof(Administration));
            }

            
        }

        [HttpGet]
        public IActionResult Administration()
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

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var model = await GetAdminDetailsViewModel(id);
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return RedirectToAction(nameof(Index));
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
                ModelState.AddModelError(string.Empty, ex.Message);
                return RedirectToAction(nameof(Details), new { id });
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
                    return RedirectToAction(nameof(Details), new { id = model.Id });
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
                ModelState.AddModelError(string.Empty, ex.Message);
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        private async Task<AdminDetailsViewModel> GetAdminDetailsViewModel(Guid id)
        {
            var admin = await _adminService.GetByIdAsync(id);
            var model = admin.Adapt<AdminDetailsViewModel>();
            model.Username = await _accountService.GetUserNameAsync(id);
            model.IsLockedOut = await _accountService.IsLockedOut(id);

            return model;
        }
    }
}
