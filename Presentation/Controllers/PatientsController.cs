using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models.Patient;
using Services.Abstractions;
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
                    await _patientService.CreateAsync(model.Dto);
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
                var model = new PatientManageViewModel(
                    await _identityService.GetUserNameAsync(id),
                    await _identityService.IsLockedOutAsync(id),
                    await _patientService.GetByIdAsync(id));

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
                var model = new PatientEditByIdViewModel(
                    await _identityService.GetUserNameAsync(id),
                    await _patientService.GetByIdAsync(id));

                return View(model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        [Authorize(Roles = Constants.AuthRoles.Admin)]
        public async Task<IActionResult> Edit(PatientEditByIdViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _patientService.UpdateAsync(model.Dto);
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
                var id = await _identityService.GetLoggedInUserId();
                var model = new PatientProfileViewModel(
                    await _identityService.GetUserNameAsync(id),
                    await _patientService.GetByIdAsync(id));

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
                    var id = await _identityService.GetLoggedInUserId();
                    await _patientService.UpdateAsync(model.Dto(id));
                    
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
                var id = await _identityService.GetLoggedInUserId();
                var model = new PatientProfileViewModel(
                    await _identityService.GetUserNameAsync(id),
                    await _patientService.GetByIdAsync(id));

                return View(model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
