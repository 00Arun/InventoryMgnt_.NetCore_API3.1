using InventoryBooks.Helper;
using InventoryBooks.Models.Common;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace InventoryBooks.ActionFilters
{
    public class SecurityFilter: IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string firstName = context.HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            string emailAddress = context.HttpContext.User.FindFirst(ClaimTypes.Email).Value;
            string userId = context.HttpContext.User.FindFirst(ClaimTypes.PrimarySid).Value;           
            string roles = context.HttpContext.User.FindFirst(ClaimTypes.Role).Value;
            string system = context.HttpContext.User.FindFirst(ClaimTypes.System).Value;
            string token = TokenManager.CreateToken(userId, firstName, emailAddress, roles);

            SecurityModel securityModel = new SecurityModel();
            securityModel.EmailAddress = emailAddress;
            securityModel.FirstName = firstName;
            securityModel.UserId = userId;
            securityModel.Roles = roles;
            securityModel.System = system;
            securityModel.Token = token;
            context.HttpContext.Items["SecurityModel"] = securityModel;
            var resultContext = await next();
        }
    }
}
