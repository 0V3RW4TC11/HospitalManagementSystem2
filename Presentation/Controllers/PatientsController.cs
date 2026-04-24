using Commands.Patient;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Attributes;
using Queries.Identity;
using Queries.Patient;
using ViewModels.Patient;
using ViewModels.User;

namespace Presentation.Controllers
{
    [Authorize(Roles = $"{Constants.AuthRoles.Admin},{Constants.AuthRoles.Patient}")]
    public class PatientsController(ISender sender) : Controller
    {
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
        public async Task<IActionResult> Create(CreateViewModel<PatientDataViewModel> model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var command = model.Adapt<CreatePatientCommand>();
                    await sender.Send(command);
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
        [Authorize(Roles = Constants.AuthRoles.Admin)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await sender.Send(new DeletePatientCommand(id));
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return RedirectToAction(nameof(Manage), new { id });
            }
        }

        [HttpGet]
        [AuthorizeAdminOrOwner]
        public async Task<IActionResult> Edit(Guid id, string returnUrl)
        {
            try
            {
                var model = await sender.Send(new GetPatientEditModel(id));
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
        public async Task<IActionResult> Edit(EditViewModel<PatientDataViewModel> model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var command = model.Adapt<UpdatePatientCommand>();
                    await sender.Send(command);
                    return Redirect(returnUrl);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = Constants.AuthRoles.Admin)]
        public async Task<IActionResult> Index(int? page)
        {
            int pageNum = page ?? 1;
            int pageSize = 10;

            try
            {
                var results = await sender.Send(new GetPatientPagedModels(pageNum, pageSize));
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
                var model = await sender.Send(new GetPatientManageModel(id));
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
                var id = await sender.Send(new GetHmsUserIdQuery());
                var model = await sender.Send(new GetPatientEditModel(id));
                return View(model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //[Authorize(Roles = Constants.AuthRoles.Doctor)]
        //[HttpGet]
        //public async Task<IActionResult> DoctorPatientIndex(int? page)
        //{
        //    try
        //    {
        //        var pagedResults = await GetPagedPatients(page);
        //        return View(pagedResults);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        //[Authorize(Roles = Constants.AuthRoles.Doctor)]
        //[HttpGet]
        //public async Task<IActionResult> DoctorPatient(Guid id)
        //{
        //    try
        //    {
        //        var model = new DoctorPatientViewModel(
        //            await _patientService.GetByIdAsync(id),
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
