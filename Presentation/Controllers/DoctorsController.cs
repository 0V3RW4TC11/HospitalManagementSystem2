using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models.Doctor;
using Services.Abstractions;
using X.PagedList.Extensions;

namespace Presentation.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly IDoctorService _doctorService;
        private readonly IIdentityService _identityService;
        private readonly ISpecializationService _specializationService;

        public DoctorsController(IServiceManager manager)
        {
            _doctorService = manager.DoctorService;
            _identityService = manager.IdentityService;
            _specializationService = manager.SpecializationService;
        }

        [HttpGet]
        [Authorize(Roles = Constants.AuthRoles.Admin)]
        public async Task<IActionResult> Create()
        {
            var specializations = await _specializationService.GetAllAsync();
            var model = new DoctorCreateViewModel(await _specializationService.GetAllAsync());
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = Constants.AuthRoles.Admin)]
        public async Task<IActionResult> Create(DoctorCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _doctorService.CreateAsync(model.Dto);
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
        [Authorize(Roles = Constants.AuthRoles.Doctor)]
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
                await _doctorService.DeleteAsync(id);
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
                var doctors = await _doctorService.Doctors(pageNum, pageSize);
                var pagedResults = doctors.List
                    .Select(p => p.Adapt<DoctorListItemViewModel>())
                    .ToPagedList(pageNum, pageSize, doctors.TotalCount);
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
                var model = new DoctorManageViewModel(
                    await _doctorService.GetByIdAsync(id),
                    await _identityService.GetUserNameAsync(id),
                    await _identityService.IsLockedOutAsync(id),
                    await _specializationService.GetAllAsync());
               
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
                var model = new DoctorEditByIdViewModel(
                    id, 
                    await _identityService.GetUserNameAsync(id), 
                    await _doctorService.GetByIdAsync(id),
                    await _specializationService.GetAllAsync());
                
                return View(model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        [Authorize(Roles = Constants.AuthRoles.Admin)]
        public async Task<IActionResult> Edit(DoctorEditByIdViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _doctorService.UpdateAsync(model.Dto);
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
        [Authorize(Roles = Constants.AuthRoles.Doctor)]
        public async Task<IActionResult> EditProfile()
        {
            try
            {
                var id = await _identityService.GetLoggedInUserId();
                
                var model = new DoctorEditProfileViewModel(
                    await _identityService.GetUserNameAsync(id),
                    await _doctorService.GetByIdAsync(id),
                    await _specializationService.GetAllAsync());
                
                return View(model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        [Authorize(Roles = Constants.AuthRoles.Doctor)]
        public async Task<IActionResult> EditProfile(DoctorEditProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var dto = model.Dto(await _identityService.GetLoggedInUserId());
                    await _doctorService.UpdateAsync(dto);
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
        [Authorize(Roles = Constants.AuthRoles.Doctor)]
        public async Task<IActionResult> Profile()
        {
            try
            {
                var id = await _identityService.GetLoggedInUserId();
                var model = new DoctorProfileViewModel(
                    await _doctorService.GetByIdAsync(id),
                    await _specializationService.GetAllAsync(),
                    await _identityService.GetUserNameAsync(id)
                );

                return View(model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
