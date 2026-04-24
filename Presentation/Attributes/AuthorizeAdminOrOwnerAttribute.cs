using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MediatR;
using Queries.Identity;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;

namespace Presentation.Attributes
{
    /// <summary>
    /// Authorization filter attribute that checks if the current user is either:
    /// 1. An admin (can modify any resource), or
    /// 2. The resource owner (can only modify their own resource)
    /// 
    /// Apply this attribute to action methods to enforce admin-or-owner authorization checks.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class AuthorizeAdminOrOwnerAttribute : Attribute, IAsyncActionFilter
    {
        /// <summary>
        /// Executes before and after the action method runs.
        /// Performs authorization checks before allowing the action to execute.
        /// </summary>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Check if the user is authenticated at all
            if (context.HttpContext.User?.Identity?.IsAuthenticated != true)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Extract the user's role from their claims
            var userRole = context.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
            bool isAdmin = userRole == Constants.AuthRoles.Admin;

            // Admins bypass all ownership checks and can proceed immediately
            if (isAdmin)
            {
                await next();
                return;
            }

            // For non-admin users, verify they own the resource being modified
            // Find the EditViewModel<T> parameter from the action method arguments
            var model = context.ActionArguments.Values.FirstOrDefault(v => v?.GetType().Name == "EditViewModel`1");

            if (model != null)
            {
                // Extract the resource ID from the model's Id property
                var idProperty = model.GetType().GetProperty("Id");
                if (idProperty != null && idProperty.GetValue(model) is Guid resourceId)
                {
                    // Get the current user's ID from the identity service via MediatR
                    var sender = context.HttpContext.RequestServices.GetRequiredService<ISender>();
                    var currentUserId = await sender.Send(new GetHmsUserIdQuery());

                    // If the user's ID matches the resource ID, they own it and can proceed
                    if (currentUserId == resourceId)
                    {
                        await next();
                        return;
                    }
                }
            }

            // User is neither an admin nor the resource owner - deny access
            context.Result = new ForbidResult();
        }
    }
}