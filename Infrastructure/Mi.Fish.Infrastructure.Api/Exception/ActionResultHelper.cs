using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Mi.Fish.Infrastructure.Api.Exception
{
    public static class ActionResultHelper2
    {
        public static bool IsViewResult(Type returnType)
        {
            //Get the actual return type (unwrap Task)
            if (returnType == typeof(Task))
            {
                returnType = typeof(void);
            }
            else if (returnType.GetTypeInfo().IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                returnType = returnType.GenericTypeArguments[0];
            }

            if (typeof(IActionResult).GetTypeInfo().IsAssignableFrom(returnType))
            {
                if (typeof(ViewResult).GetTypeInfo().IsAssignableFrom(returnType) || 
                    typeof(PartialViewResult).GetTypeInfo().IsAssignableFrom(returnType) ||
                    typeof(ViewComponentResult).GetTypeInfo().IsAssignableFrom(returnType))
                {
                    return true;
                }

                return false;
            }

            return false;
        }
    }
}