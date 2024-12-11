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
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index(int statusCode, string message)
        {
            return View(new ErrorViewModel { StatusCode = statusCode, Message = message });
        }

        public class ErrorViewModel
        {
            public int StatusCode { get; set; }
            public string Message { get; set; } = "Oops, an error occured";
        }
    }
}
