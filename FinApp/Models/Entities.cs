using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace FinApp.Models
{
    public class AppUser
    {
        public int Id { get; set; }

        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.USER;
        public string JwtToken { get; set; } = string.Empty;

        [AllowNull]
        public License License { get; set; }
    }

    public class License
    {
        [Required]
        public string Key { get; set; } = string.Empty;
        public LicenseKeyType Type { get; set; } = LicenseKeyType.BASIC;
        public DateTime Expiry { get; set; }
        public int UserId { get; set; }

        [AllowNull]
        public AppUser User { get; set; }
    }
}
