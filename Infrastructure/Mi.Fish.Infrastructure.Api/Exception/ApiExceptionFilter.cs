using System;
using System.Linq;
using System.Net;
using Abp.AspNetCore.Mvc.Extensions;
using Abp.AspNetCore.Mvc.Results;
using Abp.Authorization;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Events.Bus;
using Abp.Events.Bus.Exceptions;
using Abp.Localization;
using Abp.Logging;
using Abp.Runtime.Validation;
using Abp.Web.Models;
using Castle.Core.Logging;
using Mi.Fish.Infrastructure.Results;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Mi.Fish.Infrastructure.Api.Exception
{
    public class ApiExceptionFilter : IExceptionFilter, ITransientDependency
    {

        public ILogger Logger { get; set; }

        public IEventBus EventBus { get; set; }

        private readonly IErrorInfoBuilder _errorInfoBuilder;

        private readonly IHostingEnvironment _environment;

        private readonly ILocalizationManager _localizationManager;

        public ApiExceptionFilter(IErrorInfoBuilder errorInfoBuilder, IHostingEnvironment env,ILocalizationManager localizationManager)
        {
            _errorInfoBuilder = errorInfoBuilder;
            _environment = env;
            _localizationManager = localizationManager;

            Logger = NullLogger.Instance;
            EventBus = NullEventBus.Instance;
        }

        public void OnException(ExceptionContext context)
        {
            LogHelper.LogException(Logger, context.Exception);

            if (!context.ActionDescriptor.IsControllerAction())
            {
                return;
            }
            
            HandleAndWrapException(context);
        }

        private void HandleAndWrapException(ExceptionContext context)
        {
            //ActionResult
            //ViewResult
            //JsonResult

            if (ActionResultHelper2.IsViewResult(context.ActionDescriptor.GetMethodInfo().ReturnType))
            {
                return;
            }

            var code = GetStatusCode(context);
            context.HttpContext.Response.StatusCode = GetStatusCode(context);

            var errorInfo = _errorInfoBuilder.BuildForException(context.Exception);

            //错误信息
            
            string errorMessage;
            if (code == StatusCodes.Status500InternalServerError && _environment.IsDevelopment())
            {
                errorMessage = errorInfo.Message + context.Exception.Message;
            }
            else if (code == StatusCodes.Status500InternalServerError)
            {
                errorMessage = L("InternalServerError");
            }
            else
            {
                errorMessage = errorInfo.ValidationErrors != null && errorInfo.ValidationErrors.Any() ?
                    errorInfo.Message + " " + errorInfo.ValidationErrors.Select(a => a.Message).First() : errorInfo.Message;
            }

            context.Result = new ObjectResult(
                new Result(code == (int)HttpStatusCode.BadRequest ? ResultCode.ParameterFailed : ResultCode.Fail,
                    errorMessage));

            EventBus.Trigger(this, new AbpHandledExceptionData(context.Exception));

            context.Exception = null; //Handled!
        }

        protected virtual int GetStatusCode(ExceptionContext context)
        {
            if (context.Exception is AbpAuthorizationException)
            {
                return context.HttpContext.User.Identity.IsAuthenticated
                    ? (int)HttpStatusCode.Forbidden
                    : (int)HttpStatusCode.Unauthorized;
            }

            if (context.Exception is AbpValidationException)
            {
                return (int)HttpStatusCode.BadRequest;
            }

            if (context.Exception is EntityNotFoundException)
            {
                return (int)HttpStatusCode.NotFound;
            }

            return (int)HttpStatusCode.InternalServerError;
        }

        private string L(string name)
        {
            try
            {
                return _localizationManager.GetString(InfrastructureConsts.LocalizationSourceName, name);
            }
            catch
            {
                return name;
            }
        }
    }
}
