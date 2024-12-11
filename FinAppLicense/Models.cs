using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FinAppLicense
{
    public class LicenseModel
    {
        [Required]
        public string Key { get; set; } = string.Empty;
        public string Type { get; set; } = ""; //BASIC|PREMIUM|ENTERPRISE
        public DateTime Expiry { get; set; }
        public int UserId { get; set; }
        public string Status { get; set; } //ACTIVE|INACTIVE
        public string CreateBy { get; set; }
    }
}
