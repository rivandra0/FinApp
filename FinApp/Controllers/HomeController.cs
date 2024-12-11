using System.Diagnostics;
using FinApp.Models;
using FinApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly JwtService _jwtService;

        public HomeController(JwtService jwtService, ILogger<HomeController> logger)
        {
            _jwtService = jwtService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            string userId = _jwtService.GetTokenPayload(Request, "Id");
            string role = _jwtService.GetTokenPayload(Request, "Role");
            string fullName = _jwtService.GetTokenPayload(Request, "FullName");
            string licenseType = _jwtService.GetTokenPayload(Request, "LicenseType");

            ViewBag.UserId = userId;
            ViewBag.Role = role;
            ViewBag.FullName = fullName;
            ViewBag.LicenseType = licenseType;

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
}
