using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace FinAppLicense.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LicenseController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly string _tokenSecret;

        public LicenseController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet]
        [Route("GetAll")]
        public IActionResult GetMany()
        {
            using var connection = new SqlConnection(_connectionString);
            var sql =
                "SELECT [Key], [Type], Expiry, UserId, [Status], CreatedBy, CreatedAt FROM License";
            var licenses = connection.Query<LicenseModel>(sql).ToList();

            return Ok(licenses);
        }

        [HttpGet]
        [Route("Get/{licenseKey}")]
        public IActionResult GetOne(string licenseKey)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql =
                "SELECT [Key], [Type], Expiry, UserId, [Status], CreatedBy, CreatedAt FROM License WHERE [Key] = @Key";
            var license = connection.QuerySingleOrDefault<LicenseModel>(
                sql,
                new { Key = licenseKey }
            );

            if (license == null)
                return NotFound(new { message = "License not found" });

            return Ok(license);
        }

        [HttpGet]
        [Route("GetByUser/{userId}")]
        public IActionResult GetOneByUserId(int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql =
                @"
                SELECT TOP 1 [Key], [Type], Expiry, UserId, [Status], CreatedBy, CreatedAt
                FROM License
                WHERE UserId = @UserId
                ORDER BY 
                    CASE [Type]
                        WHEN 'ENTERPRISE' THEN 1
                        WHEN 'PREMIUM' THEN 2
                        WHEN 'BASIC' THEN 3
                        ELSE 4
                    END";
            var license = connection.QuerySingleOrDefault<LicenseModel>(
                sql,
                new { UserId = userId }
            );

            if (license == null)
                return NotFound(new { message = "License not found" });

            return Ok(license);
        }

        [HttpPost]
        [Route("Insert")]
        public IActionResult InsertOne([FromBody] LicenseModel license)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql =
                @"
                INSERT INTO License([Key], [Type], Expiry, UserId, [Status], CreatedBy)
                VALUES (@Key, @Type, NULL, NULL, @Status, @CreatedBy)";
            var parameters = new
            {
                license.Key,
                license.Type,
                license.Status,
                license.CreateBy,
            };

            connection.Execute(sql, parameters);

            return Ok(new { message = "License successfully inserted" });
        }

        [HttpPost]
        [Route("Activate")]
        public IActionResult ActivateOne([FromBody] ActivateLicenseRequest request)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql =
                @"
                UPDATE License
                SET Expiry = DATEADD(YEAR, 1, GETDATE()), 
                    UserId = @UserId,
                    [Status] = 'ACTIVE' 
                WHERE [Key] = @Key";
            var parameters = new { request.LicenseKey, request.UserId };

            connection.Execute(sql, parameters);

            return Ok(new { message = "License successfully activated" });
        }
    }

    public class ActivateLicenseRequest
    {
        public string LicenseKey { get; set; }
        public int UserId { get; set; }
    }
}
