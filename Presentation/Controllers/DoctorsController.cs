using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models.Doctor;
using Services.Abstractions;
using Services.Dtos.Doctor;
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
            var model = new DoctorCreateViewModel 
            { 
                EditSpecViewModels = GetEditSpecViewModels()
            };
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
                    var dto = model.DetailsViewModel.Adapt<DoctorCreateDto>();
                    dto.Password = model.PasswordViewModel.Password;
                    dto.SpecializationIds = GetSelectedSpecIds(model.EditSpecViewModels);
                    await _doctorService.CreateAsync(dto);
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
                var model = await GetDoctorManageViewModel(id);
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
                var dto = await _doctorService.GetByIdAsync(id);
                var specializations = await _specializationService.GetAllAsync();
                var model = new DoctorEditViewModel
                {
                    Id = id,
                    UserName = await _identityService.GetUserNameAsync(id),
                    DetailsViewModel = dto.Adapt<DoctorDetailsViewModel>(),
                    EditSpecViewModels = GetEditSpecViewModels(dto.SpecializationIds)
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
        public async Task<IActionResult> Edit(DoctorEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var dto = model.DetailsViewModel.Adapt<DoctorDto>();
                    dto.Id = model.Id;
                    dto.SpecializationIds = GetSelectedSpecIds(model.EditSpecViewModels);
                    await _doctorService.UpdateAsync(dto);
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
                var userId = await _identityService.GetLoggedInUserId();
                var model = await GetDoctorEditProfileViewModel(userId);
                return View(model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // TODO: Separate specializations update from doctor update
        [HttpPost]
        [Authorize(Roles = Constants.AuthRoles.Doctor)]
        public async Task<IActionResult> EditProfile(DoctorEditProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userId = await _identityService.GetLoggedInUserId();
                    var oldDto = await _doctorService.GetByIdAsync(userId);
                    var newDto = model.DetailsViewModel.Adapt<DoctorDto>();
                    newDto.Id = userId;
                    newDto.SpecializationIds = oldDto.SpecializationIds;
                    await _doctorService.UpdateAsync(newDto);
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
                var userId = await _identityService.GetLoggedInUserId();
                var model = await GetDoctorProfileViewModel(userId);
                return View(model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task<DoctorManageViewModel> GetDoctorManageViewModel(Guid id)
        {
            var dto = await _doctorService.GetByIdAsync(id);
            var detailsViewModel = dto.Adapt<DoctorDetailsViewModel>();
            var specsViewModel = await GetDoctorSpecsViewModel(dto.SpecializationIds);

            return new DoctorManageViewModel
            {
                Id = id,
                Username = await _identityService.GetUserNameAsync(id),
                IsLockedOut = await _identityService.IsLockedOut(id),
                DetailsViewModel = detailsViewModel,
                SpecsViewModel = specsViewModel
            };
        }

        private async Task<DoctorProfileViewModel> GetDoctorProfileViewModel(Guid id)
        {
            var dto = await _doctorService.GetByIdAsync(id);
            var detailsViewModel = dto.Adapt<DoctorDetailsViewModel>();
            var specsViewModel = await GetDoctorSpecsViewModel(dto.SpecializationIds);
            
            return new DoctorProfileViewModel
            {
                Username = await _identityService.GetUserNameAsync(id),
                DetailsViewModel = detailsViewModel,
                SpecsViewModel = specsViewModel
            };
        }

        private async Task<DoctorEditProfileViewModel> GetDoctorEditProfileViewModel(Guid id)
        {
            var dto = await _doctorService.GetByIdAsync(id);
            var detailsViewModel = dto.Adapt<DoctorDetailsViewModel>();
            return new DoctorEditProfileViewModel
            {
                UserName = await _identityService.GetUserNameAsync(id),
                DetailsViewModel = detailsViewModel
            };
        }

        private async Task<DoctorSpecsViewModel> GetDoctorSpecsViewModel(IEnumerable<Guid> specIds)
        {
            var dtos = await _specializationService.GetAllAsync();
            var specs = dtos
                .Where(s => specIds.Contains(s.Id))
                .Select(s => s.Name);

            return new DoctorSpecsViewModel
            {
                Specializations = specs
            };
        }

        private HashSet<Guid> GetSelectedSpecIds(IEnumerable<DoctorEditSpecViewModel> models)
        {
            return models
                .Where(s => s.IsSelected)
                .Select(s => s.Id)
                .ToHashSet();
        }

        private DoctorEditSpecViewModel[] GetEditSpecViewModels()
        {
            var specializations = _specializationService.GetAllAsync().Result;

            return specializations
                    .Select(s => new DoctorEditSpecViewModel
                    {
                        Id = s.Id,
                        Name = s.Name,
                        IsSelected = false
                    })
                    .ToArray();
        }

        private DoctorEditSpecViewModel[] GetEditSpecViewModels(IEnumerable<Guid>? specIds)
        {
            var specializations = _specializationService.GetAllAsync().Result;

            return specializations
                    .Select(s => new DoctorEditSpecViewModel
                    {
                        Id = s.Id,
                        Name = s.Name,
                        IsSelected = specIds.Contains(s.Id)
                    })
                    .ToArray();
        }
    }
}
