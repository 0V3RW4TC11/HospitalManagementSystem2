using Commands.Doctor;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Attributes;
using Queries.Doctor;
using Queries.Identity;
using Queries.Shared;
using ViewModels.Doctor;

namespace Presentation.Controllers
{
    [Authorize(Roles = $"{Constants.AuthRoles.Admin},{Constants.AuthRoles.Doctor}")]
    public class DoctorsController(ISender sender) : Controller
    {
        [HttpGet]
        [Authorize(Roles = Constants.AuthRoles.Admin)]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = Constants.AuthRoles.Admin)]
        public async Task<IActionResult> Create(CreateDoctorViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var specIds = model.SpecializationsJson.GetSpecializationIdsFromJson();
                    var doctorData = model.Data.Adapt<DoctorData>() with { SpecializationIds = specIds };
                    await sender.Send(new CreateDoctorCommand(doctorData, model.PasswordModel.Password));
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    return View(model);
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Dashboard()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = Constants.AuthRoles.Admin)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await sender.Send(new DeleteDoctorCommand(id));
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return RedirectToAction(nameof(Manage), new { id });
            }
        }

        [HttpGet]
        [Authorize(Roles = Constants.AuthRoles.Admin)]
        public async Task<IActionResult> Index(int? page)
        {
            int pageNum = page ?? 1;
            int pageSize = 10;

            try
            {
                var results = await sender.Send(new GetPagedModels<DoctorIndexViewModel>(pageNum, pageSize));
                return View(results);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        [Authorize(Roles = Constants.AuthRoles.Admin)]
        public async Task<IActionResult> Manage(Guid id)
        {
            try
            {
                var model = await sender.Send(new GetDoctorManageModel(id));
                return View(model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        [AuthorizeAdminOrOwner]
        public async Task<IActionResult> Edit(Guid id, string returnUrl)
        {
            try
            {
                var model = await sender.Send(new GetDoctorEditModel(id));
                ViewData["ReturnUrl"] = returnUrl;
                return View(model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        [AuthorizeAdminOrOwner]
        public async Task<IActionResult> Edit(EditDoctorViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var specIds = model.SpecializationsJson.GetSpecializationIdsFromJson();
                    var doctorData = model.Data.Adapt<DoctorData>() with { SpecializationIds = specIds };
                    await sender.Send(new UpdateDoctorCommand(model.Id, doctorData));
                    return Redirect(returnUrl);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    return View(model);
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            try
            {
                var id = await sender.Send(new GetHmsUserIdQuery());
                var model = await sender.Send(new GetDoctorProfileModel(id));
                return View(model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
