using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mi.Fish.Infrastructure.Results;
using Microsoft.AspNetCore.Mvc;

namespace Mi.Fish.Infrastructure.Api
{
    public static class InvalidModelStateExecutor
    {
        public static Func<ActionContext, IActionResult> Executer = (context) =>
        {
            var firstErrors = context.ModelState.First(o => o.Value.Errors.Any()).Value.Errors;

            var errorMessage = string.Join(" ", firstErrors.Select(o => o.ErrorMessage));

            return new BadRequestObjectResult(new Result(ResultCode.ParameterFailed, errorMessage));
        };
    }
}
