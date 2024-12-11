namespace FinApp.Core
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class LicenseProtectAttribute : Attribute
    {
        public string[] Types { get; }

        public LicenseProtectAttribute(params string[] licenseTypes)
        {
            Types = licenseTypes;
        }
    }
}
