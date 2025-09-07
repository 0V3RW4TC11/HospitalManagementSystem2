using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Helpers;
using Presentation.Models.Patient;
using Services.Abstractions;
using Services.Dtos.Patient;
using X.PagedList.Extensions;

namespace Presentation.Controllers
{
    public class PatientsController : Controller
    {
        private readonly IPatientService _patientService;
        private readonly IIdentityService _identityService;

        public PatientsController(IServiceManager manager)
        {
            _patientService = manager.PatientService;
            _identityService = manager.IdentityService;
        }

        [HttpGet]
        [Authorize(Roles = Constants.AuthRoles.Admin)]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = Constants.AuthRoles.Admin)]
        public async Task<IActionResult> Create(PatientCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var dto = model.Adapt<PatientCreateDto>();
                    await _patientService.CreateAsync(dto);
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
        [Authorize(Roles = Constants.AuthRoles.Patient)]
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
                await _patientService.DeleteAsync(id);
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
                var patients = await _patientService.Patients(pageNum, pageSize);
                var pagedAdmins = patients.List
                    .Select(p => p.Adapt<PatientListItemViewModel>())
                    .ToPagedList(pageNum, pageSize, patients.TotalCount);
                return View(pagedAdmins);
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
                var model = await GetPatientDetailsViewModel(id);
                return View(model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        [Authorize(Roles = Constants.AuthRoles.Admin)]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var model = await GetPatientDetailsViewModel(id);
                return View(model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        [Authorize(Roles = Constants.AuthRoles.Admin)]
        public async Task<IActionResult> Edit(PatientManageViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var dto = model.Adapt<PatientDto>();
                    await _patientService.UpdateAsync(dto);
                    return RedirectToAction(nameof(Manage), new { id = model.Id });
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
        [Authorize(Roles = Constants.AuthRoles.Patient)]
        public async Task<IActionResult> EditProfile()
        {
            try
            {
                var userId = await _identityService.GetLoggedInUserId();
                var model = await GetPatientProfileViewModel(userId);
                return View(model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        [Authorize(Roles = Constants.AuthRoles.Patient)]
        public async Task<IActionResult> EditProfile(PatientProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var patientDto = model.Adapt<PatientDto>();
                    patientDto.Id = await _identityService.GetLoggedInUserId();
                    await _patientService.UpdateAsync(patientDto);
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
        [Authorize(Roles = Constants.AuthRoles.Patient)]
        public async Task<IActionResult> Profile()
        {
            try
            {
                var userId = await _identityService.GetLoggedInUserId();
                var model = await GetPatientDetailsViewModel(userId);
                return View(model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task<PatientManageViewModel> GetPatientDetailsViewModel(Guid id)
        {
            var patient = await _patientService.GetByIdAsync(id);
            var model = patient.Adapt<PatientManageViewModel>();
            model.Username = await _identityService.GetUserNameAsync(id);
            model.IsLockedOut = await _identityService.IsLockedOut(id);
            return model;
        }

        private async Task<PatientProfileViewModel> GetPatientProfileViewModel(Guid id)
        {
            var patient = await _patientService.GetByIdAsync(id);
            var model = patient.Adapt<PatientProfileViewModel>();
            model.Username = await _identityService.GetUserNameAsync(id);
            return model;
        }
    }
}
