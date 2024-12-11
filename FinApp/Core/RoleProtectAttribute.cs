using System;

namespace FinApp.Core
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RoleProtectAttribute : Attribute
    {
        public string[] Roles { get; }

        public RoleProtectAttribute(params string[] roles)
        {
            Roles = roles;
        }
    }
}
