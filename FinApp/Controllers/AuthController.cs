using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using FinApp.Core;
using FinApp.Data;
using FinApp.Models;
using FinApp.Services;
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
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            AppUser user = _context.AppUser.GetOne(model.Email);

            if (user == null)
                throw new HttpException(StatusCodes.Status404NotFound, "user not found");

            bool isVerify = BCrypt.Net.BCrypt.Verify(model.Password, user.Pwd);
            if (!isVerify)
                throw new HttpException(StatusCodes.Status400BadRequest, "wrong password");

            string generatedtoken = _jwtservice.GeneratePageAccessToken(user);
            _ = generatedtoken;
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            AppUser user = _context.AppUser.GetOne(model.Email);
            if (user != null)
            {
                throw new HttpException(StatusCodes.Status400BadRequest, "user already exists");
            }

            string hashedPwd = BCrypt.Net.BCrypt.HashPassword(model.Password);
            AppUser registeredUser = _context.AppUser.InsertOne(model.Email, hashedPwd, model.FullName);
            _ = registeredUser;

            return Redirect("RegisterSuccess");
        }

        [HttpGet]
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
