using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models.Attendance;
using Services.Abstractions;
using X.PagedList.Extensions;

namespace Presentation.Controllers
{
    public class AttendancesController : Controller
    {
        private readonly IAttendanceService _attendanceService;
        private readonly IDoctorService _doctorService;
        private readonly IIdentityService _identityService;
        private readonly IPatientService _patientService;

        public AttendancesController(IServiceManager manager)
        {
            _attendanceService = manager.AttendanceService;
            _patientService = manager.PatientService;
            _identityService = manager.IdentityService;
            _doctorService = manager.DoctorService;
        }

        [Authorize(Roles = Constants.AuthRoles.Doctor)]
        [HttpGet]
        public async Task<IActionResult> AttendancesByPatientId(Guid id, int? page)
        {
            int pageNum = page ?? 1;
            int pageSize = 10;

            try
            {
                var patientUserName = await _identityService.GetUserNameAsync(id);

                var model = new AttendanceResultsViewModel
                {
                    ListName = "Attendances for " + patientUserName,
                    ResultName = "Doctor",
                    ReturnRoute = nameof(AttendancesByPatientId),
                    RouteValues = (page) => new { id, page }
                };

                var attendances = await _attendanceService.FindAttendancesByPatientPagedAsync(
                    id,
                    pageNum,
                    pageSize);

                var resultModels = new List<AttendanceResultViewModel>();
                foreach (var attendance in attendances.List)
                {
                    var doctor = await _doctorService.GetByIdAsync(attendance.DoctorId); // TODO: use batching
                    resultModels.Add(new AttendanceResultViewModel(
                        attendance.AttendanceId,
                        attendance.DateTime,
                        string.Empty,
                        doctor.FirstName,
                        doctor.LastName));
                }
                model.PagedResults = resultModels
                    .ToPagedList(pageNum, pageSize, attendances.TotalCount);

                return View("Attendances", model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [Authorize(Roles = Constants.AuthRoles.Doctor)]
        [HttpGet]
        public async Task<IActionResult> Create(Guid patientId)
        {
            var doctorId = await _identityService.GetLoggedInUserId();
            var patient = await _patientService.GetByIdAsync(patientId);
            var model = new AttendanceCreateViewModel(doctorId, patient);
            return View(model);
        }

        [Authorize(Roles = Constants.AuthRoles.Doctor)]
        [HttpPost]
        public async Task<IActionResult> Create(AttendanceCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _attendanceService.CreateAsync(model.Dto);
                    return RedirectToAction(nameof(PatientsController.DoctorPatient), "Patients", new { id = model.PatientId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            return View(model);
        }

        [Authorize(Roles = $"{Constants.AuthRoles.Doctor}, {Constants.AuthRoles.Admin}")]
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _attendanceService.DeleteAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Details(Guid id, string? returnUrl)
        {
            try
            {
                var attendance = await _attendanceService.GetByIdAsync(id);
                var userId = await _identityService.GetLoggedInUserId();
                if (User.IsInRole(Constants.AuthRoles.Patient) && attendance.PatientId != userId)
                {
                    return Forbid();
                }

                var model = new AttendanceDetailsViewModel(
                    attendance,
                    await _doctorService.GetByIdAsync(attendance.DoctorId),
                    await _patientService.GetByIdAsync(attendance.PatientId),
                    returnUrl ?? Url.Action("Home", nameof(HomeController.Index))!);

                return View(model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [Authorize(Roles = Constants.AuthRoles.Doctor)]
        [HttpGet]
        public async Task<IActionResult> DoctorAttendances(int? page)
        {
            int pageNum = page ?? 1;
            int pageSize = 10;

            var model = new AttendanceResultsViewModel
            {
                ListName = "My Attendances",
                ResultName = "Patient",
                ReturnRoute = nameof(DoctorAttendances),
                RouteValues = (page) => new { page }
            };

            try
            {
                var attendances = await _attendanceService.FindAttendancesByDoctorPagedAsync(
                    await _identityService.GetLoggedInUserId(),
                    pageNum,
                    pageSize);

                var resultModels = new List<AttendanceResultViewModel>();
                foreach (var attendance in attendances.List)
                {
                    var patient = await _patientService.GetByIdAsync(attendance.PatientId); // TODO: use batching
                    resultModels.Add(new AttendanceResultViewModel(
                        attendance.AttendanceId,
                        attendance.DateTime,
                        patient.Title,
                        patient.FirstName,
                        patient.LastName));
                }
                model.PagedResults = resultModels
                    .ToPagedList(pageNum, pageSize, attendances.TotalCount);

                return View("Attendances", model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [Authorize(Roles = Constants.AuthRoles.Patient)]
        [HttpGet]
        public async Task<IActionResult> PatientAttendances(int? page)
        {
            int pageNum = page ?? 1;
            int pageSize = 10;

            var model = new AttendanceResultsViewModel
            {
                ListName = "My Attendances",
                ResultName = "Doctor",
                ReturnRoute = nameof(PatientAttendances),
                RouteValues = (page) => new { page }
            };

            try
            {
                var attendances = await _attendanceService.FindAttendancesByPatientPagedAsync(
                    await _identityService.GetLoggedInUserId(),
                    pageNum,
                    pageSize);

                var resultModels = new List<AttendanceResultViewModel>();
                foreach (var attendance in attendances.List)
                {
                    var doctor = await _doctorService.GetByIdAsync(attendance.DoctorId); // TODO: use batching
                    resultModels.Add(new AttendanceResultViewModel(
                        attendance.AttendanceId,
                        attendance.DateTime,
                        string.Empty,
                        doctor.FirstName,
                        doctor.LastName));
                }
                model.PagedResults = resultModels
                    .ToPagedList(pageNum, pageSize, attendances.TotalCount);

                return View("Attendances", model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
