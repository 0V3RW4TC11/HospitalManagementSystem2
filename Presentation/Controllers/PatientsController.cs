using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
        private readonly IAccountService _accountService;

        public PatientsController(IServiceManager manager)
        {
            _patientService = manager.PatientService;
            _accountService = manager.AccountService;
        }

        [Authorize(Roles = Constants.AuthRoles.Admin)]
        [HttpGet]
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

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Create(string? returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Create(PatientCreateViewModel model, string? returnUrl)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var dto = model.Adapt<PatientCreateDto>();
                    await _patientService.CreateAsync(dto);
                    return UrlHelper.Redirect(this, returnUrl);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    return View(model);
                }
            }
            return View(model);
        }

        [Authorize(Roles = Constants.AuthRoles.Admin)]
        public async Task<IActionResult> Details(Guid id)
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
        public async Task<IActionResult> Edit(Guid id, string? returnUrl)
        {
            try
            {
                ViewData["ReturnUrl"] = returnUrl;
                var model = await GetPatientDetailsViewModel(id);
                return View(model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PatientDetailsViewModel model, string? returnUrl)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var dto = model.Adapt<PatientDto>();
                    await _patientService.UpdateAsync(dto);
                    return UrlHelper.Redirect(this, returnUrl);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    return View(model);
                }
            }
            return View(model);
        }

        private async Task<PatientDetailsViewModel> GetPatientDetailsViewModel(Guid id)
        {
            var patient = await _patientService.GetByIdAsync(id);
            var model = patient.Adapt<PatientDetailsViewModel>();
            model.Username = await _accountService.GetUserNameAsync(id);
            model.IsLockedOut = await _accountService.IsLockedOut(id);
            return model;
        }
    }
}
