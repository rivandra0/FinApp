using System.ComponentModel;
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
    public class AuthController : Controller
    {
        private readonly ILogger<AuthController> _logger;
        private readonly DbContext _context;
        private readonly JwtService _jwtservice;

        public AuthController(DbContext context, JwtService jwtService, ILogger<AuthController> logger)
        {
            _logger = logger;
            _context = context;
            _jwtservice = jwtService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            AppUserModel user = _context.AppUser.GetOne(model.Email);

            if (user == null)
            {
                ViewBag.ErrorMessage = "user not found";
                return View(model);
                //throw new HttpException(StatusCodes.Status404NotFound, "user not found");
            }

            LicenseModel license = _context.License.GetOneByUserId(user.Id);

            if (license == null)
            {
                license = new LicenseModel
                {
                    Key = "",
                    Type = "",
                    Expiry = DateTime.Now.AddYears(1000),
                };
            }

            user.License = license;

            bool isVerify = BCrypt.Net.BCrypt.Verify(model.Password, user.Pwd);
            if (!isVerify)
            {
                ViewBag.ErrorMessage = "wrong password";
                return View(model);
                //throw new HttpException(StatusCodes.Status400BadRequest, "wrong password");
            }

            string generatedtoken = _jwtservice.GeneratePageAccessToken(user);
            Response.Cookies.Append(
                "jwttoken",
                generatedtoken,
                new CookieOptions
                {
                    HttpOnly = true, // Helps mitigate XSS attacks
                    Secure = true, // Set to true in production to enforce HTTPS
                    SameSite = SameSiteMode.Strict, // Prevents CSRF attacks
                    Expires = DateTime.UtcNow.AddHours(1),
                }
            );
            ViewBag.SuccessMessage = $"successfully logged in, welcome {user.FullName}";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            AppUserModel user = _context.AppUser.GetOne(model.Email);
            if (user != null)
            {
                throw new HttpException(StatusCodes.Status400BadRequest, "user already exists");
            }

            string hashedPwd = BCrypt.Net.BCrypt.HashPassword(model.Password);
            AppUserModel registeredUser = _context.AppUser.InsertOne(model.Email, hashedPwd, model.FullName);
            _ = registeredUser;

            return Redirect("RegisterSuccess");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegisterSuccess()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public class LoginViewModel
        {
            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid email address")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Password is required")]
            [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public class RegisterViewModel
        {
            [Required(ErrorMessage = "Full Name is required")]
            [StringLength(100, ErrorMessage = "Full Name cannot exceed 100 characters")]
            public string FullName { get; set; }

            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid email address")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Password is required")]
            [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }
    }
}
