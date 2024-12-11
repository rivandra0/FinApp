using System.Diagnostics;
using FinApp.Core;
using FinApp.Models;
using FinApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinApp.Controllers
{
    public class FeatureController : Controller
    {
        private readonly ILogger<FeatureController> _logger;

        public FeatureController(JwtService jwtService, ILogger<FeatureController> logger)
        {
            _logger = logger;
        }

        [LicenseProtect("BASIC", "PREMIUM", "ENTERPRISE")]
        public IActionResult FeatureBasic()
        {
            return View();
        }

        [LicenseProtect("PREMIUM", "ENTERPRISE")]
        public IActionResult FeaturePremium()
        {
            return View();
        }

        [LicenseProtect("ENTERPRISE")]
        public IActionResult FeatureEnterprise()
        {
            return View();
        }
    }
}
