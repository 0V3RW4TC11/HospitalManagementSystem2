using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
                    var dto = model.DetailsViewModel.Adapt<PatientCreateDto>();
                    dto.Password = model.PasswordViewModel.Password;
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
                var pagedResults = patients.List
                    .Select(p => p.Adapt<PatientListItemViewModel>())
                    .ToPagedList(pageNum, pageSize, patients.TotalCount);
                return View(pagedResults);
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
                var model = new PatientManageViewModel
                {
                    Id = id,
                    UserName = await _identityService.GetUserNameAsync(id),
                    IsLockedOut = await _identityService.IsLockedOutAsync(id),
                    DetailsViewModel = await GetPatientDetailsViewModel(id)
                };

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
                var model = new PatientEditViewModel
                {
                    Id = id,
                    UserName = await _identityService.GetUserNameAsync(id),
                    DetailsViewModel = await GetPatientDetailsViewModel(id)
                };

                return View(model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        [Authorize(Roles = Constants.AuthRoles.Admin)]
        public async Task<IActionResult> Edit(PatientEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var dto = model.DetailsViewModel.Adapt<PatientDto>();
                    dto.Id = model.Id;
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
                var model = new PatientEditProfileViewModel
                {
                    UserName = await _identityService.GetUserNameAsync(userId),
                    DetailsViewModel = await GetPatientDetailsViewModel(userId)
                };

                return View(model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        [Authorize(Roles = Constants.AuthRoles.Patient)]
        public async Task<IActionResult> EditProfile(PatientEditProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var dto = model.DetailsViewModel.Adapt<PatientDto>();
                    dto.Id = await _identityService.GetLoggedInUserId();
                    await _patientService.UpdateAsync(dto);
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
                var model = new PatientProfileViewModel
                {
                    UserName = await _identityService.GetUserNameAsync(userId),
                    DetailsViewModel = await GetPatientDetailsViewModel(userId)
                };

                return View(model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task<PatientDetailsViewModel> GetPatientDetailsViewModel(Guid id)
        {
            var patient = await _patientService.GetByIdAsync(id);
            return patient.Adapt<PatientDetailsViewModel>();
        }
    }
}
