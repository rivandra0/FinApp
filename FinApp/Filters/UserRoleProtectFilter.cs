using System.Linq;
using FinApp.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class RoleProtectFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        // Get the current user's roles (retrieve from claims or session)
        var userRoles = GetUserRolesFromContext(context);

        // Get the RoleProtect attribute
        var roleProtectAttribute = context.ActionDescriptor.EndpointMetadata.OfType<HttpException>().FirstOrDefault();

        if (roleProtectAttribute != null)
        {
            // Check if the user has any of the required roles
            if (!roleProtectAttribute.Roles.Any(role => userRoles.Contains(role)))
            {
                // If not, return a 403 Forbidden
                context.Result = new ForbidResult();
            }
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // No action needed after execution
    }

    private string[] GetUserRolesFromContext(ActionExecutingContext context)
    {
        // Example: Retrieve roles from user claims
        var user = context.HttpContext.User;
        return user
            .Claims.Where(c => c.Type == "role") // Replace "role" with your claim type
            .Select(c => c.Value)
            .ToArray();
    }
}
