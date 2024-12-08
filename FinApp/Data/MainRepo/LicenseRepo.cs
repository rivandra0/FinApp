using FinApp.Models;

namespace FinApp.Data.MainRepo
{
    public interface ILicenseRepo
    {
        ///<summary>
        /// Inserting one new license, only admin can do this
        ///</summary>
        string InsertOne(License license);

        ///<summary>
        /// Get one license key by license key and user id.
        ///</summary>
        string GetOne(string licensekey, string userid);

        ///<summary>
        /// Get all license key and see the owner of it, the expiry, etc.
        ///</summary>
        string GetMany();
    }

    public class LicenseRepo : ILicenseRepo
    {
        public string _ConnectionString { get; set; }

        public LicenseRepo(string connstr)
        {
            _ConnectionString = connstr;
        }

        public string GetMany()
        {
            throw new NotImplementedException();
        }

        public string GetOne(string licensekey, string userid)
        {
            throw new NotImplementedException();
        }

        public string InsertOne(License license)
        {
            throw new NotImplementedException();
        }
    }
}
