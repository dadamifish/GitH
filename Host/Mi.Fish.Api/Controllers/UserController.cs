using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Abp.Runtime.Caching;
using Mi.Fish.Application;
using Mi.Fish.ApplicationDto;
using Mi.Fish.Infrastructure.Api.JwtBearer;
using Mi.Fish.Infrastructure.Results;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Mi.Fish.Api.Controllers
{
    /// <summary>
    /// 用户登录
    /// </summary>
    public class UserController : FishControllerBase
    {
        public const string LoginRouteName = "Login";

        private readonly IUserService _userService;

        private readonly JwtSetting _jwtSetting;

        private readonly ICacheManager _cacheManager;

        public UserController(IUserService userService, IOptions<JwtSetting> jwtOptions, ICacheManager cacheManager)
        {
            _userService = userService;
            _cacheManager = cacheManager;
            _jwtSetting = jwtOptions.Value;
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="userLoginInput"></param>
        /// <returns></returns>
        /// <response code="200">请求成功</response>
        /// <response code="400">请求失败</response>
        [ProducesResponseType(typeof(UserLoginOutPut), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [HttpPost("Login", Name = LoginRouteName)]
        [AllowAnonymous]
        public IActionResult UserLogin([FromBody]UserLoginInput userLoginInput)
        {
            var result = _userService.UserLogin(userLoginInput);

            if (result.IsSuccess)
            {

                //生成用户token
                var token = JwtToken.GetToken(_jwtSetting, result.Data.UserID, result.Data.StorageNo, result.Data.TerminalID);
                //缓存token
                _cacheManager.SetUserTokenCache(result.Data.UserID, token);

                return Ok(new UserLoginOutPut{ Token = token , UserName = result.Data.UserName });
            }

            return BadRequest(result.BaseResult());
        }

        /// <summary>
        /// 用户退出
        /// </summary>
        /// <returns></returns>
        /// <response code="200">请求成功</response>
        /// <response code="400">请求失败</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [HttpGet("LogOut")]
        public async Task<IActionResult> UserLogOut()
        {
            string userId = FishSession.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("错误的用户信息。");
            }

            var result =await _userService.UserLogOut(userId);
            if (result.IsSuccess)
            {
                //清理用户Token
                _cacheManager.RemoveUserTokenCache(userId);
                return Ok();
            }

            return BadRequest(result.BaseResult());
        }

        /// <summary>
        /// 用户密码修改
        /// </summary>
        /// <returns></returns>
        /// <response code="200">请求成功</response>
        /// <response code="400">请求失败</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [HttpPatch("Password")]
        public async Task<IActionResult> UserPwdUpdate([FromBody]UserPwdUpdateInput userPwdUpdateInput)
        {
            var result = await _userService.UserPwdUpdate(userPwdUpdateInput);
            if (result.IsSuccess)
            {
                return Ok();
            }

            return BadRequest(result.BaseResult());
        }



        /// <summary>
        /// 用户交班
        /// </summary>
        /// <returns></returns>
        /// <response code="200">请求成功</response>
        /// <response code="400">请求失败</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [HttpGet("TurnClass")]
        public async Task<IActionResult> UserTurnClass()
        {
            var result = await _userService.UserTurnClass();
            if (result.IsSuccess)
            {
                return Ok();
            }

            return BadRequest(result.BaseResult());
        }

        /// <summary>
        /// 用户交班明细
        /// </summary>
        /// <returns></returns>
        /// <response code="200">请求成功</response>
        /// <response code="400">请求失败</response>
        [ProducesResponseType(typeof(TurnClassDetailOutPut), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [HttpGet("TurnClassDetail")]
        public async Task<IActionResult> GetTurnClassDetail()
        {
            var result = await _userService.GetUserTurnClassDetail();
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.BaseResult());
        }

    }
}
