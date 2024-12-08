using System;
using FinApp.Data.MainRepo;

namespace FinApp.Data
{
    public class DbContext
    {
        public DbContext(string connectionString)
        {
            _appUserRepo = new Lazy<AppUserRepo>(() => new AppUserRepo(connectionString));
            _licenseRepo = new Lazy<LicenseRepo>(() => new LicenseRepo(connectionString));
        }

        // Lazy loaded repositories
        private readonly Lazy<AppUserRepo> _appUserRepo;
        public AppUserRepo AppUser => _appUserRepo.Value;

        private readonly Lazy<LicenseRepo> _licenseRepo;
        public LicenseRepo Package => _licenseRepo.Value;
    }
}
