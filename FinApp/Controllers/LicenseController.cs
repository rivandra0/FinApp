using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using FinApp.Core;
using FinApp.Data;
using FinApp.Models;
using FinApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinApp.Controllers
{
    public class LicenseController : Controller
    {
        private readonly ILogger<LicenseController> _logger;
        private readonly DbContext _context;
        private readonly JwtService _jwtService;

        public LicenseController(DbContext context, JwtService jwtService, ILogger<LicenseController> logger)
        {
            _logger = logger;
            _context = context;
            _jwtService = jwtService;
        }

        [RoleProtect("SUPERADMIN", "ADMIN")]
        [HttpGet]
        public IActionResult Index()
        {
            var vm = new LicenseListViewModel { Licenses = _context.License.GetMany() };
            return View(vm);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Activate()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ActivateSuccss()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Activate(LicenseActivateViewModel vm)
        {
            int userid = Convert.ToInt32(_jwtService.GetTokenPayload(Request, "Id"));
            string key = vm.Key;

            LicenseModel lcFromDb = _context.License.GetOne(key);
            if (lcFromDb.Status == "ACTIVE")
            {
                return RedirectToAction("Index", "Error", new { statusCode = 500, message = "license already activated by a user" });
            }

            if (lcFromDb == null)
            {
                return RedirectToAction("Index", "Error", new { statusCode = 400, message = "wrong key" });
            }

            string res = _context.License.ActivateOne(key, userid);

            return View("ActivateSuccess");
        }

        [HttpPost]
        public IActionResult Create(LicenseCreateViewModel vm)
        {
            var license = new LicenseModel
            {
                Key = _GenerateLicenseKey(),
                Type = vm.Type,
                Status = "INACTIVE",
                UserId = Convert.ToInt32(_jwtService.GetTokenPayload(Request, "Id")),
            };

            LicenseModel lcFromDb;
            do
            {
                lcFromDb = _context.License.GetOne(license.Key);
                if (lcFromDb != null)
                {
                    license.Key = _GenerateLicenseKey();
                }
            } while (lcFromDb != null);

            _context.License.InsertOne(license);

            return RedirectToAction("Index");
        }

        string _GenerateLicenseKey()
        {
            string guidString = Guid.NewGuid().ToString("N");
            return guidString.Substring(0, 20).ToUpper(); // Take first 20 characters
        }

        public class LicenseListViewModel
        {
            public List<LicenseModel> Licenses { get; set; }
        }

        public class LicenseCreateViewModel
        {
            [Required]
            public string Type { get; set; } = ""; //BASIC|PREMIUM|ENTERPRISE
        }

        public class LicenseActivateViewModel
        {
            [Required]
            [RegularExpression(@"^[A-Za-z0-9]{20}$", ErrorMessage = "The Key must be exactly 20 alphanumeric characters.")]
            public string Key { get; set; } = string.Empty;
        }
    }
}
