using Commands.Specialization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Queries.Shared;
using Queries.Specialization;
using ViewModels.Specialization;

namespace Presentation.Controllers
{
    [Authorize]
    public class SpecializationsController(ISender sender) : Controller
    {
        [Authorize(Roles = Constants.AuthRoles.Admin)]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = Constants.AuthRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> Create(string name)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await sender.Send(new CreateSpecializationCommand(name));
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(nameof(Create), name);
        }

        [Authorize(Roles = Constants.AuthRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await sender.Send(new DeleteSpecializationCommand(id));
                return Ok();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = Constants.AuthRoles.Admin)]
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var model = await sender.Send(new GetEditSpecModel(id));
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        [Authorize(Roles = Constants.AuthRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> Edit(SpecViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await sender.Send(new UpdateSpecializationCommand(model.Id, model.Name));
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(model);
        }

        [Authorize(Roles = Constants.AuthRoles.Admin)]
        [HttpGet]
        public async Task<IActionResult> Index(int? page)
        {
            int pageNum = page ?? 1;
            int pageSize = 10;

            try
            {
                var pagedModels = await sender.Send(new GetPagedModels<SpecViewModel>(pageNum, pageSize));
                return View(pagedModels);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SpecViewModel>>> Search(string name)
        {
            try
            {
                var specializations = await sender.Send(new FindSpecsByName(name));
                return Ok(specializations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
