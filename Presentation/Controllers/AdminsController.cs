using Commands.Admin;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Queries.Admin;

namespace Presentation.Controllers
{
    [Authorize(Roles = Constants.AuthRoles.Admin)]
    public class AdminsController : Controller
    {
        private readonly ISender _sender;

        public AdminsController(ISender sender) => _sender = sender;

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

        //[HttpPost]
        //public async Task<IActionResult> Create(CreateAdminViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            await _sender.Send(model.Command);
        //            return RedirectToAction(nameof(Index));
        //        }
        //        catch (Exception ex)
        //        {
        //            ModelState.AddModelError(string.Empty, ex.Message);
        //        }
        //    }
        //    return View(model);
        //}

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _sender.Send(new DeleteAdminCommand(id));
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //[HttpGet]
        //public async Task<IActionResult> Edit(Guid id)
        //{
        //    try
        //    {
        //        var model = new AdminManageViewModel(
        //            await _adminService.GetByIdAsync(id),
        //            await _identityService.GetUserNameAsync(id),
        //            await _identityService.IsLockedOutAsync(id));
        //        return View(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        //[HttpPost]
        //public async Task<IActionResult> Edit(ManageAdminViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            await _adminService.UpdateAsync(model.Dto);
        //            return RedirectToAction(nameof(Manage), new { id = model.Id});
        //        }
        //        catch (Exception ex)
        //        {
        //            ModelState.AddModelError(string.Empty, ex.Message);
        //        }
        //    }

        //    return View(model);
        //}

        //[HttpGet]
        //public async Task<IActionResult> EditProfile()
        //{
        //    try
        //    {
        //        var id = await _identityService.GetLoggedInUserId();
        //        var model = new AdminProfileViewModel(
        //            await _adminService.GetByIdAsync(id),
        //            await _identityService.GetUserNameAsync(id));
        //        return View(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        //[HttpPost]
        //public async Task<IActionResult> EditProfile(ProfileAdminViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var id = await _identityService.GetLoggedInUserId();
        //            await _adminService.UpdateAsync(model.Dto(id));
        //            return RedirectToAction(nameof(Profile));
        //        }
        //        catch (Exception ex)
        //        {
        //            throw;
        //        }
        //    }

        //    return View(model);
        //}

        [HttpGet]
        public async Task<IActionResult> Index(int? page)
        {
            int pageNum = page ?? 1;
            int pageSize = 10;

            try
            {
                var results = await _sender.Send(new GetPagedAdminsQuery(pageNum, pageSize));
                return View(results);
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
                var model = await _sender.Send(new GetManageAdminQuery(id));
                return View(model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //[HttpGet]
        //public async Task<IActionResult> Profile()
        //{
        //    try
        //    {
        //        var id = await _identityService.GetLoggedInUserId();

        //        var model = new AdminProfileViewModel(
        //            await _adminService.GetByIdAsync(id),
        //            await _identityService.GetUserNameAsync(id));

        //        return View(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}
    }
}
