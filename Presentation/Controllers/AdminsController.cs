using Domain.Constants;
using Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;
using X.PagedList.Extensions;

namespace Presentation.Controllers
{
    [Authorize(Roles = AuthRoles.Admin)]
    public class AdminsController : Controller
    {
        private readonly IAdminRepository _adminRepository;

        public AdminsController(IRepositoryManager manager)
        {
            _adminRepository = manager.AdminRepository;
        }

        public async Task<IActionResult> Index(int? page)
        {
            int pageSize = 10;
            int pageNum = page ?? 1;

            var admins = await _adminRepository.GetAdminListAsync();
            var models = admins.ToArray()
                        .Select(a => new AdminListItemViewModel 
                        { 
                            Id = a.Id, 
                            FirstName = a.FirstName, 
                            LastName = a.LastName, 
                            Email = a.Email 
                        })
                        .ToPagedList(pageNum, pageSize);

            return View(models);
        }

        public IActionResult Administration()
        {
            return View();
        }
    }
}
