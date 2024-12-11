using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace FinApp.Models
{
    public class AppUserModel
    {
        public int Id { get; set; }

        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string Pwd { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; }
        public string Status { get; set; }
        public string JwtToken { get; set; } = string.Empty;
        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedAt { get; set; }

        [AllowNull]
        public LicenseModel License { get; set; }
    }

    public class LicenseModel
    {
        [Required]
        public string Key { get; set; } = string.Empty;
        public string Type { get; set; } = ""; //BASIC|PREMIUM|ENTERPRISE
        public DateTime Expiry { get; set; }
        public int UserId { get; set; }
        public string Status { get; set; } //ACTIVE|INACTIVE

        [AllowNull]
        public AppUserModel User { get; set; }
    }
}
