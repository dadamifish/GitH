using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Controllers;
using Abp.Web.Models;
using Mi.Fish.ApplicationDto;
using Mi.Fish.Infrastructure;
using Mi.Fish.Infrastructure.Session;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mi.Fish.Api.Controllers
{
    [Produces("application/json")]
    [DontWrapResult]
    [Route("api/[controller]")]
    //[ApiController]
    //[Authorize("POS")]
    [EnableCors(FishApiConsts.DefaultPolicy)]
    public abstract class FishControllerBase : AbpController
    {
        protected FishControllerBase()
        {
            LocalizationSourceName = InfrastructureConsts.LocalizationSourceName;
        }

        public IFishSession FishSession { get; set; }

        /// <summary>
        /// 获取IP地址
        /// </summary>
        /// <returns></returns>
        protected string GetUserIp()
        {
            var ip = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(ip))
            {
                ip = HttpContext.Connection.RemoteIpAddress.ToString();
            }
            return ip;
        }
    }
}
