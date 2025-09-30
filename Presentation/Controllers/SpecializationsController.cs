using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Services.Dtos.Specialization;
using X.PagedList.Extensions;

namespace Presentation.Controllers
{
    public class SpecializationsController : Controller
    {
        private readonly ISpecializationService _specializationService;

        public SpecializationsController(IServiceManager manager)
        {
            _specializationService = manager.SpecializationService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(SpecializationCreateDto dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _specializationService.CreateAsync(dto);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(dto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _specializationService.DeleteAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var model = await _specializationService.GetByIdAsync(id);
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(SpecializationDto dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _specializationService.UpdateAsync(dto);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(dto);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Index(int? page)
        {
            int pageNum = page ?? 1;
            int pageSize = 10;

            try
            {
                var specs = await _specializationService.SpecializationsPaged(pageNum, pageSize);
                return View(specs.List.ToPagedList(pageNum, pageSize, specs.TotalCount));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SpecializationDto>>> Search(string name)
        {
            try
            {
                var specializations = await _specializationService.SearchByNameAsync(name);
                return Ok(specializations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
