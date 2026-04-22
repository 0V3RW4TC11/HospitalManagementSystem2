using Commands.Admin;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Queries.Admin;
using Queries.Identity;
using ViewModels.Admin;
using ViewModels.User;

namespace Presentation.Controllers
{
    [Authorize(Roles = Constants.AuthRoles.Admin)]
    public class AdminsController : Controller
    {
        private readonly ISender _sender;

        public AdminsController(ISender sender) => _sender = sender;

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
        public async Task<IActionResult> Create(CreateViewModel<DataViewModel> model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var command = model.Adapt<CreateAdminCommand>();
                    await _sender.Send(command);
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
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _sender.Send(new DeleteAdminCommand(id));
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id, string returnUrl)
        {
            try
            {
                var model = await _sender.Send(new GetEditAdminModel(id));
                ViewData["ReturnUrl"] = returnUrl;
                return View(model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditViewModel<DataViewModel> model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var command = model.Adapt<UpdateAdminCommand>();
                    await _sender.Send(command);
                    return Redirect(returnUrl);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? page)
        {
            int pageNum = page ?? 1;
            int pageSize = 10;

            try
            {
                var results = await _sender.Send(new GetPagedAdminsQuery(pageNum, pageSize));
                return View(results);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> Manage(Guid id)
        {
            try
            {
                var model = await _sender.Send(new GetManageAdminModel(id));
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
                var id = await _sender.Send(new GetHmsUserIdQuery());
                var model = await _sender.Send(new GetEditAdminModel(id));
                return View(model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
