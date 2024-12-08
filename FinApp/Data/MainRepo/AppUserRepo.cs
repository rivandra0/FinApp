using FinApp.Models;

namespace FinApp.Data.MainRepo
{
    public interface IAuthRepo
    {
        ///<summary>
        /// get one user by email and password
        ///</summary>
        AppUser GetOne(string email, string password);

        ///<summary>
        /// get many users only for admin
        ///</summary>
        AppUser GetMany(string licensetype, string userrole);

        ///<summary>
        /// Inserting one user
        ///</summary>
        AppUser InsertOne(string email, string password, string fullname);
    }

    public class AppUserRepo : IAuthRepo
    {
        public string _ConnectionString { get; set; }

        public AppUserRepo(string connstr)
        {
            _ConnectionString = connstr;
        }

        public AppUser GetOne(string email, string password)
        {
            //fetch one user by id and password
            //if not exists then throw exception
            //return the user
            throw new NotImplementedException();
        }

        public AppUser InsertOne(string email, string password, string fullname)
        {
            //check does email exists
            //if exists then throw exception
            //set userrole to be USER
            //use bcrpyt for hashing here
            //then insert
            throw new NotImplementedException();
        }

        public AppUser GetMany(string licensetype, string userrole)
        {
            //get users by the licensetype and the role
            //return
            throw new NotImplementedException();
        }
    }
}
