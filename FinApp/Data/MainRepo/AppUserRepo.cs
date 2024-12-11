using Dapper;
using FinApp.Core;
using FinApp.Models;
using Microsoft.Data.SqlClient;

namespace FinApp.Data.MainRepo
{
    public interface IAuthRepo
    {
        ///<summary>
        /// get one user by email and password
        ///</summary>
        AppUserModel GetOne(string email);

        ///<summary>
        /// get many users only for admin
        ///</summary>
        AppUserModel GetMany(string licensetype, string userrole);

        ///<summary>
        /// Inserting one user
        ///</summary>
        AppUserModel InsertOne(string email, string password, string fullname);
    }

    public class AppUserRepo : IAuthRepo
    {
        public string _ConnectionString { get; set; }

        public AppUserRepo(string connstr)
        {
            _ConnectionString = connstr;
        }

        public AppUserModel GetOne(string email)
        {
            using var connection = new SqlConnection(_ConnectionString);

            var parameters = new DynamicParameters();
            parameters.Add("Email", email);

            var sql =
                @"
                    SELECT  
                        Top 1
                        Id,
                        Email,
                        FullName,
                        Pwd,
                        [Role],
                        [Status],
                        CreatedBy,
                        CreatedAt,
                        UpdatedBy,
                        UpdatedAt
                    FROM AppUser
                    WHERE Email=@Email";

            var user = connection.QueryFirstOrDefault<AppUserModel>(sql, parameters);

            return user;
        }

        public AppUserModel InsertOne(string email, string hashedpwd, string fullname)
        {
            using var connection = new SqlConnection(_ConnectionString);

            var parameters = new AppUserModel
            {
                Email = email,
                Pwd = hashedpwd,
                FullName = fullname,
                Role = "USER",
                Status = "VERIFIED",
                CreatedBy = "SYSTEM",
            };

            var sql =
                @"
                    INSERT INTO AppUser(Email, FullName, Pwd, [Role], [Status], CreatedBy)
                    VALUES(@Email, @FullName, @Pwd, @Role, @Status, @CreatedBy)";

            connection.Execute(sql, parameters);

            return parameters;
        }

        public AppUserModel GetMany(string licensetype, string userrole)
        {
            //get users by the licensetype and the role
            //return
            throw new NotImplementedException();
        }
    }
}
