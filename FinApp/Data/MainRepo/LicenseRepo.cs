using System.ComponentModel;
using Dapper;
using FinApp.Models;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Server;

namespace FinApp.Data.MainRepo
{
    public interface ILicenseRepo
    {
        ///<summary>
        /// Inserting one new license, only admin can do this
        ///</summary>
        string InsertOne(LicenseModel license);

        ///<summary>
        /// Get one license key by license key and user id.
        ///</summary>
        LicenseModel GetOne(string licensekey);
        LicenseModel GetOneByUserId(int userid);

        ///<summary>
        /// Get all license key and see the owner of it, the expiry, etc.
        ///</summary>
        List<LicenseModel> GetMany();

        ///<summary>
        /// Activating one license for a user.
        ///</summary>
        string ActivateOne(string licensekey, int userid);
    }

    public class LicenseRepo : ILicenseRepo
    {
        public string _ConnectionString { get; set; }

        public LicenseRepo(string connstr)
        {
            _ConnectionString = connstr;
        }

        public List<LicenseModel> GetMany()
        {
            using var connection = new SqlConnection(_ConnectionString);

            var parameters = new DynamicParameters();

            var sql =
                @"
                    select [key], [Type], Expiry, UserId, [Status], CreatedBy, CreatedAt
                    from License
                ";

            List<LicenseModel> licenses = connection.Query<LicenseModel>(sql, parameters).ToList();

            return licenses;
        }

        public LicenseModel GetOne(string licensekey)
        {
            using var connection = new SqlConnection(_ConnectionString);

            var parameters = new DynamicParameters();
            parameters.Add("Key", licensekey);

            var sql =
                @"
                    select [Key], [Type], Expiry, UserId, [Status], CreatedBy, CreatedAt
                    from License where [Key]=@Key
                ";

            LicenseModel license = connection.QuerySingleOrDefault<LicenseModel>(sql, parameters);

            return license;
        }

        public LicenseModel GetOneByUserId(int userid)
        {
            using var connection = new SqlConnection(_ConnectionString);

            var parameters = new DynamicParameters();
            parameters.Add("UserId", userid);

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
                            ELSE 4 -- Optional: for unexpected values
                        END
                ";

            LicenseModel license = connection.QuerySingleOrDefault<LicenseModel>(sql, parameters);

            return license;
        }

        public string ActivateOne(string licensekey, int userid)
        {
            using var connection = new SqlConnection(_ConnectionString);

            var parameters = new DynamicParameters();
            parameters.Add("Key", licensekey);
            parameters.Add("UserId", userid);

            var sql =
                @"
                    UPDATE License
                    SET Expiry = DATEADD(year, 1, GETDATE()), 
                        UserId = @UserId,
                        [Status] = 'ACTIVE' 
                    WHERE [Key] = @Key;
                    ";

            connection.Execute(sql, parameters);

            return "successfully activate";
        }

        public string InsertOne(LicenseModel license)
        {
            using var connection = new SqlConnection(_ConnectionString);

            var parameters = new DynamicParameters();
            parameters.Add("Key", license.Key);
            parameters.Add("Type", license.Type);
            parameters.Add("Status", license.Status);
            parameters.Add("UserId", license.UserId);

            var sql =
                @"
                    Insert into License([key], [Type], Expiry, UserId, [Status], CreatedBy)
                    VALUES (@Key, @Type, null, null, @Status, @UserId);
                ";

            var res = connection.Execute(sql, parameters);

            return "successfuly insert license";
        }
    }
}
