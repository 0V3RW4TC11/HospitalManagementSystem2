using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Services.Dtos.Specialization;

namespace Presentation.Controllers
{
    public class SpecializationsController : Controller
    {
        private readonly ISpecializationService _specializationService;

        public SpecializationsController(IServiceManager manager)
        {
            _specializationService = manager.SpecializationService;
        }

        public IActionResult Index()
        {
            return View();
        }

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
