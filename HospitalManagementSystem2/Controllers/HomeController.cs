using System.Diagnostics;
using HospitalManagementSystem2.Models;
using Microsoft.AspNetCore.Mvc;
<<<<<<<< HEAD:Presentation/Controllers/HomeController.cs
using Microsoft.Extensions.Logging;
using Presentation.Models;

namespace Presentation.Controllers;
========

namespace HospitalManagementSystem2.Controllers;
>>>>>>>> origin/main:HospitalManagementSystem2/Controllers/HomeController.cs

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}