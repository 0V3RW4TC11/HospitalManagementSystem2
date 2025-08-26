using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;
using Services.Abstractions;
using X.PagedList.Extensions;
using Mapster;

namespace Presentation.Controllers
{
    [Authorize(Roles = AuthRoles.Admin)]
    public class AdminsController : Controller
    {
        private readonly IAdminService _adminService;

        public AdminsController(IServiceManager manager)
        {
            _adminService = manager.AdminService;
        }

        public async Task<IActionResult> Index(int? page)
        {
            int pageNum = page ?? 1;
            int pageSize = 10;

            var admins = await _adminService.GetAdminsAsync(pageNum, pageSize);
            var pagedAdmins = admins.List
                .Select(a => a.Adapt<AdminListItemViewModel>())
                .ToPagedList(pageNum, pageSize, admins.TotalCount);

            return View(pagedAdmins);
        }

        public IActionResult Administration()
        {
            return View();
        }
    }
}
