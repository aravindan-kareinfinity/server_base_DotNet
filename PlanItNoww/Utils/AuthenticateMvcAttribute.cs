﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PlanItNoww.Models;

namespace PlanItNoww.Utils
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class AuthenticateMvcAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            //var user = (Users)context.HttpContext.Items["User"];
            //if (user == null)
            //{
            //    context.Result = new RedirectResult("/user/login");
            //    return;
            //}
        }
    }
}
